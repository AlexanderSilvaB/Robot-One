#include <robotOne.h>
#include <iostream>
#include <KFs/EKF.hpp>
#include <vector>
#include <cmath>
#include <fstream>
#include <opencv2/opencv.hpp>

using namespace std;
using namespace cv;

void model(Matrix<double, 3, 1> &x, Matrix<double, 2, 1> &u, double dt)
{
	Matrix<double, 3, 1> dx;
	dx << cos(x(2)) * u(0) * dt,
		sin(x(2)) * u(0) * dt,
		u(1) * dt;
	x = x + dx;
}

void sensor(Matrix<double, 2, 1> &y, Matrix<double, 3, 1> &x, Matrix<double, 2, 1> &d, double dt)
{
	double dx, dy;
	dx = d(0) - x(0);
	dy = d(1) - x(1);
	y << sqrt(dx * dx + dy * dy),
		atan2(dy, dx) - x(2);
}

void modelJ(Matrix<double, 3, 3> &F, Matrix<double, 3, 1> &x, Matrix<double, 2, 1> &u, double dt)
{
	F << 1, 0, -sin(x(2)) * u(0) * dt,
		0, 1, cos(x(2)) * u(0) * dt,
		0, 0, 1;
}

void sensorJ(Matrix<double, 2, 3> &H, Matrix<double, 3, 1> &x, Matrix<double, 2, 1> &d, double dt)
{
	double dx, dy, ds, dv, dn1, dn2;
	dx = d(0) - x(0);
	dy = d(1) - x(1);
	ds = sqrt(dx * dx + dy * dy);
	dv = dy / dx;
	dn1 = 1 + (dv * dv) * (dx * dx);
	dn2 = 1 + (dv * dv) * dx;

	H << -dx / ds, -dy / ds, 0,
		dy / dn1, -1 / dn2, -1;
}

int main(int argc, char *argv[])
{
	// Connects to Robot One
	int handler = connectRobotOne("127.0.0.1");
	if (!handler)
	{
		cout << "Could not connect to Robot One" << endl;
		return -1;
	}

	// Show the robot trace
	setTrace(false);

	// Select some velocities
	float Vlinear = 3;
	//float Vangular = 0.1;
	float Vangular = 0;

	// Initializes the camera data structure
	CameraData cameraData;
	memset(&cameraData, '\0', sizeof(CameraData));
	initCamera(&cameraData);

	// Sets the Extended Kalman Filter
	double sigma_x_x = 0.001;
	double sigma_x_y = 0.001;
	double sigma_x_a = 0.0001;
	double sigma_y_r = 0.1;
	double sigma_y_b = 0.2;

	// Gets the initial position
	Value3 pose;
	getPose(&pose);

	Matrix<double, 3, 1> x0;
	x0 << (double)pose.values[0], (double)pose.values[1], (double)pose.values[2];

	EKF<double, 3, 2, 2, 2> ekf(x0);
	ekf.setModel(model);
	ekf.setSensor(sensor);
	ekf.setModelJacobian(modelJ);
	ekf.setSensorJacobian(sensorJ);
	ekf.seed();

	auto Q = ekf.createQ();
	Q << sigma_x_x * sigma_x_x, 0, 0,
		0, sigma_x_y*sigma_x_y, 0,
		0, 0, sigma_x_a*sigma_x_a;
	ekf.setQ(Q);

	auto R = ekf.createR();
	R << sigma_y_r * sigma_y_r, 0,
		0, sigma_y_b*sigma_y_b;
	ekf.setR(R);

	// Landmarks-------
	/*
	C 14 4 1 1
	C 19 -4 1 1
	C 26 4 1 1
	C 35 -4 1 1
	*/
	auto D = ekf.createData();
	D << 14, 4;
	ekf.addData(D);
	D << 19, -4;
	ekf.addData(D);
	D << 26, 4;
	ekf.addData(D);
	D << 35, -4;
	ekf.addData(D);
	// ----------------

	// Kalman filter variables
	auto xK = ekf.state();
	auto u = ekf.input();
	u << Vlinear, Vangular;
	vector< Matrix<double, 2, 1> > ys;
	vector<double> X, Y, XK, YK;

	// Image processing variables
	const double FMM = 37;
	const double BOX_H = 1000;
	const double SENSOR_H = 40;
	const double FOV = 30;

	Mat rgb, bgr, mask;
	rgb = Mat::zeros(cameraData.height, cameraData.width, CV_8UC3);
	Scalar box_color_min(50, 100, 20);
	Scalar box_color_max(70, 180, 140);

	// Sets the odometry errors
	Value2 odometryStd;
	odometryStd.values[0] = sigma_x_x;
	odometryStd.values[1] = sigma_x_a;
	setOdometryStd(&odometryStd);

	// Sets the initial position
	Value3 pose0;
	pose0.values[0] = x0(0);
	pose0.values[1] = x0(1);
	pose0.values[2] = x0(2);
	setPose(&pose0);

	// Sets the velocities
	Value2 velocity;
	velocity.values[0] = Vlinear;
	velocity.values[1] = Vangular;
	setVelocity(&velocity);

	// Run for T seconds
	double T = 30;
	double dt = 0;
	double t = 0;

	ofstream file("plot.txt");

	while (t < T)
	{
		// Get a new camera frame
		int sz = captureCamera(&cameraData);
		if (sz == cameraData.size)
		{
			memcpy(rgb.data, cameraData.data, cameraData.size);

			// Fix the image for opencv
			cvtColor(rgb, bgr, COLOR_RGB2BGR);
			flip(bgr, bgr, 0);

			// Create the mask image
			inRange(bgr, box_color_min, box_color_max, mask);

			// Find boxes
			ys.clear();
			vector<vector<Point> > contours;
			findContours(mask, contours, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE);
			for (int i = 0; i < contours.size(); i++)
			{
				Rect r = boundingRect(contours[i]);
				double a = r.area();
				if (a < 20)
					continue;
				int cx = r.x + r.width / 2;
				int cy = r.y + r.height / 2;
				
				double bearing = FOV * 0.0174533 * ( (cx - 160.0) / 160.0 );
				double range = ( (FMM*BOX_H*cameraData.height) / (r.height*SENSOR_H) ) / 1000.0;

				auto y = ekf.output();
				y << range, bearing;
				ys.push_back(y);
				//cout << "Y(" << i << ") = " << y << endl;
			}

			// Show the image
			imshow("image", mask);
			waitKey(1);
		}

		// Run the filter
		dt = ekf.time();
		ekf.run(xK, ys, u, dt);
		XK.push_back(xK(0));
		YK.push_back(xK(1));

		cout << "XK: " << xK(0) << ", " << xK(1) << endl;

		// Gets the current pose
		getPose(&pose);
		X.push_back(pose.values[0]);
		Y.push_back(pose.values[1]);

		cout << "X: " << pose.values[0] << ", " << pose.values[1] << endl;

		// writes the data to a file
		file << pose.values[0] << ", " << pose.values[1] << ", " << xK(0) << ", " << xK(1) << endl;

		// Waits a new frame of the simulator
		waitRobotOne();

		t += dt;
	}

	// Disable trace
	setTrace(false);
	// Disconnects from the simulator
	disconnectRobotOne();
	return 0;
}
