#include <robotOne/robotOne.h>
#include <iostream>
#include <bflib/EKF.hpp>
#include <vector>
#include <cmath>
#include <fstream>
#include <sstream>
#include <opencv2/opencv.hpp>

#ifdef USE_DLIB
#include <dlib/opencv.h>
#include <dlib/image_processing.h>
#include <dlib/data_io.h>
#endif

using namespace std;
using namespace cv;
#ifdef USE_DLIB
using namespace dlib;
#endif

typedef EKF<double, 3, 2, 2, 2> Robot;

void model(Robot::State &x, Robot::Input &u, double dt)
{
	Robot::State dx;
	dx << cos(x(2)) * u(0) * dt,
		sin(x(2)) * u(0) * dt,
		u(1) * dt;
	x = x + dx;
}

void sensor(Robot::Output &y, Robot::State &x, Robot::Data &d, double dt)
{
	double dx, dy;
	dx = d(0) - x(0);
	dy = d(1) - x(1);
	y << sqrt(dx * dx + dy * dy),
		atan2(dy, dx) - x(2);
}

void modelJ(Robot::ModelJacobian &F, Robot::State &x, Robot::Input &u, double dt)
{
	F << 1, 0, -sin(x(2)) * u(0) * dt,
		0, 1, cos(x(2)) * u(0) * dt,
		0, 0, 1;
}

void sensorJ(Robot::SensorJacobian &H, Robot::State &x, Robot::Data &d, double dt)
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
#ifdef USE_DLIB
	string trainingFile = "boxes.svm";
	if(argc > 1)
	{
		trainingFile = argv[1];
	}
	cout << "Training file: " << trainingFile << endl;
#endif

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
	float Vlinear = 1;
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

	Robot::State x0;
	x0 << (double)pose.values[0], 
		  (double)pose.values[1], 
		  (double)pose.values[2];

	EKF<double, 3, 2, 2, 2> ekf(x0);
	ekf.setModel(model);
	ekf.setSensor(sensor);
	ekf.setModelJacobian(modelJ);
	ekf.setSensorJacobian(sensorJ);
	ekf.seed();

	Robot::ModelCovariance Q;
	Q << sigma_x_x * sigma_x_x, 0, 0,
		0, sigma_x_y*sigma_x_y, 0,
		0, 0, sigma_x_a*sigma_x_a;
	ekf.setQ(Q);

	Robot::SensorCovariance R;
	R << sigma_y_r * sigma_y_r, 0,
		0, sigma_y_b*sigma_y_b;
	ekf.setR(R);

	// Landmarks-------
	// C 14 4 1 1
	// C 19 -4 1 1
	// C 26 4 1 1
	// C 35 -4 1 1
	Robot::Data D;
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
	Robot::State xK;
	Robot::Input u;
	u << Vlinear, Vangular;

	std::vector< Robot::Output > ys;
	std::vector<double> X, Y, XK, YK;

	// Image processing variables
	const double FMM = 37;
	const double BOX_H = 1500;
	const double SENSOR_H = 40;
	const double FOV = 30;

	Mat rgb, bgr;
	rgb = Mat::zeros(cameraData.height, cameraData.width, CV_8UC3);
#ifdef USE_DLIB
	// Dlib
	typedef scan_fhog_pyramid<pyramid_down<6> > image_scanner_type;
	image_scanner_type scanner;
	object_detector<image_scanner_type> detector;
	deserialize(trainingFile) >> detector;
#else
	Mat mask;
	Scalar box_color_min(50, 100, 20);
	Scalar box_color_max(70, 180, 140);
#endif

	// Sets the odometry errors
	Value2 odometryStd;
	odometryStd.values[0] = sigma_x_x;
	odometryStd.values[1] = sigma_x_a;
	setOdometryStd(&odometryStd);

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

			// Find boxes
			ys.clear();

			cout << "Detecting..." << endl;
#ifdef USE_DLIB
			cv_image<bgr_pixel> cimg(bgr);
			std::vector<dlib::rectangle> boxes = detector(cimg);
#else
			// Create the mask image
			inRange(bgr, box_color_min, box_color_max, mask);
			vector<vector<Point> > boxes;
			findContours(mask, boxes, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE);
#endif
			cout << "Detected: " << boxes.size() << endl;

			for (int i = 0; i < boxes.size(); i++)
			{
#ifdef USE_DLIB
				dlib::rectangle dlibr = boxes[i];
				cv::Rect r(dlibr.left(), dlibr.top(), dlibr.width(), dlibr.height());
#else
				Rect r = boundingRect(boxes[i]);
#endif

				double a = r.area();
				if (a < 20)
					continue;
				int cx = r.x + r.width / 2;
				int cy = r.y + r.height / 2;

				double bearing = -FOV * 0.0174533 * ((cx - 160.0) / 160.0);
				double range = ((FMM*BOX_H*cameraData.height) / (r.height*SENSOR_H)) / 1000.0;

				Robot::Output y;
				y << range, bearing;
				//cout << y << endl;
				ys.push_back(y);


				cv::rectangle(bgr, r, Scalar(0, 0, 255));
			}

			// Show the image
			imshow("image", bgr);
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

	// Disconnects from the simulator
	disconnectRobotOne();
	return 0;
}
