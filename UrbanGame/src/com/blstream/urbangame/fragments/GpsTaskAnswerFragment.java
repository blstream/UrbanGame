package com.blstream.urbangame.fragments;

import android.app.Activity;
import android.app.Dialog;
import android.content.Context;
import android.location.Location;
import android.location.LocationManager;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;

import com.actionbarsherlock.app.SherlockFragment;
import com.blstream.urbangame.R;
import com.blstream.urbangame.database.Database;
import com.blstream.urbangame.database.DatabaseInterface;
import com.blstream.urbangame.database.entity.PlayerTaskSpecific;
import com.blstream.urbangame.database.entity.Task;
import com.blstream.urbangame.dialogs.AnswerDialog;
import com.blstream.urbangame.dialogs.AnswerDialog.DialogType;
import com.blstream.urbangame.listeners.GpsLocationListener;
import com.blstream.urbangame.webserver.mock.MockWebServer;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesClient.ConnectionCallbacks;
import com.google.android.gms.common.GooglePlayServicesClient.OnConnectionFailedListener;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.location.LocationClient;
import com.google.android.gms.location.LocationRequest;

public class GpsTaskAnswerFragment extends SherlockFragment implements OnClickListener, ConnectionCallbacks,
	OnConnectionFailedListener {
	
	private Task task;
	private PlayerTaskSpecific playerTaskSpecific;
	private Activity context;
	private Button submitButton;
	private GpsLocationListener gpsLocationListener;
	private static final int GPS_UPDATE_TIME = 3000;
	private final String TAG = GpsTaskAnswerFragment.class.getSimpleName();
	private boolean hasGoogleServices;
	private LocationClient mLocationClient;
	private LocationRequest mLocationRequest;
	private boolean mUpdatesRequested;
	
	@Override
	public void onAttach(Activity activity) {
		super.onAttach(activity);
		
		context = activity;
		
		task = getArguments().getParcelable(Task.TASK_KEY);
		
		//Check for google play services
		// Getting status
		int status = GooglePlayServicesUtil.isGooglePlayServicesAvailable(context.getBaseContext());
		
		// Showing status
		if (status == ConnectionResult.SUCCESS) {
			hasGoogleServices = true;
			Log.i(TAG, "has google play services");
		}
		else {
			Log.i(TAG, "no google play services");
			hasGoogleServices = false;
			int requestCode = 10;
			Dialog dialog = GooglePlayServicesUtil.getErrorDialog(status, context, requestCode);
			dialog.show();
		}
		
		gpsLocationListener = new GpsLocationListener();
		DatabaseInterface database = new Database(activity);
		playerTaskSpecific = database.getPlayerTaskSpecific(task.getId(), database.getLoggedPlayerID());
		database.closeDatabase();
		
	}
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		
		if (hasGoogleServices) {
			mLocationClient = new LocationClient(context, this, this);
		}
		
		super.onCreate(savedInstanceState);
	}
	
	@Override
	public void onStart() {
		Log.i(TAG, "onStart");
		if (hasGoogleServices) {
			mLocationClient.connect();
			
			// Create the LocationRequest object
			mLocationRequest = LocationRequest.create();
			// Use high accuracy
			mLocationRequest.setPriority(LocationRequest.PRIORITY_HIGH_ACCURACY);
			mLocationRequest.setInterval(GPS_UPDATE_TIME);
		}
		super.onStart();
	}
	
	@Override
	public void onStop() {
		if (hasGoogleServices) {
			Log.i(TAG, "onStop");
			if (mLocationClient.isConnected()) {
				mLocationClient.removeLocationUpdates(gpsLocationListener);
			}
			
			mLocationClient.disconnect();
		}
		super.onStop();
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		View view = null;
		
		view = inflater.inflate(R.layout.gps_task_answer, container, false);
		
		submitButton = (Button) view.findViewById(R.id.buttonGpsTaskAnswerSubmit);
		submitButton.setOnClickListener(this);
		return view;
	}
	
	//Get actual position and send it
	@Override
	public void onClick(View v) {
		if (hasGoogleServices) {
			AnswerDialog dialog = new AnswerDialog(context);
			
			//I failed to find how to check if gps is off using LocationClient, please tell me if you know how to do it
			LocationManager locationManager = (LocationManager) context.getSystemService(context.LOCATION_SERVICE);
			//gps is off
			if (!locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER)) {
				dialog.showDialog(DialogType.GPS_OFF);
			}
			//no gps signal
			else if (gpsLocationListener.getLastLocation() == null || !gpsLocationListener.isGPSFix()) {
				dialog.showDialog(DialogType.NO_GPS_SIGNAL);
			}
			//no network connection
			else if (!isOnline()) {
				dialog.showDialog(DialogType.NO_INTERNET_CONNECTION);
			}
			//everything went ok, we are sending data to server
			else {
				
				Location location = gpsLocationListener.getLastLocation();
				
				int result = sendLocationForVeryfication(location);
				
				int maxPoints = task.getMaxPoints();
				
				if (result == maxPoints) {
					dialog.showDialog(DialogType.RIGHT_ANSWER, result, maxPoints);
				}
				else if (result == 0) {
					dialog.showDialog(DialogType.WRONG_ANSWER, result, maxPoints);
				}
				else {
					dialog.showDialog(DialogType.PARTIALLY_RIGHT_ANSWER, result, maxPoints);
				}
				
				//this shouldn't happen in future, this if is to test it before we have playerTaskSpecyfic in db
				if (playerTaskSpecific == null) {
					playerTaskSpecific = new PlayerTaskSpecific();
					playerTaskSpecific.setTaskID(task.getId());
				}
				
				playerTaskSpecific.setPoints(result);
				playerTaskSpecific.setIsFinishedByUser(true);
				
				DatabaseInterface database = new Database(context);
				database.updatePlayerTaskSpecific(playerTaskSpecific);
				database.closeDatabase();
			}
		}
	}
	
	private int sendLocationForVeryfication(Location location) {
		
		//FIXME get real server data
		MockWebServer mockWebServer = new MockWebServer();
		return mockWebServer.sendGPSLocation(context, task, location);
	}
	
	//check internet connection
	public boolean isOnline() {
		ConnectivityManager connectivityManager = (ConnectivityManager) context
			.getSystemService(Context.CONNECTIVITY_SERVICE);
		NetworkInfo networkInfo = connectivityManager.getActiveNetworkInfo();
		if (networkInfo == null) return false;
		if (!networkInfo.isConnected()) return false;
		if (!networkInfo.isAvailable()) return false;
		return true;
	}
	
	@Override
	public void onConnected(Bundle arg0) {
		
		mLocationClient.requestLocationUpdates(mLocationRequest, gpsLocationListener);
		Log.i(TAG, "onConnected");
	}
	
	@Override
	public void onDisconnected() {
		Log.i(TAG, "onDisconnected");
	}
	
	@Override
	public void onConnectionFailed(ConnectionResult result) {
		Log.i(TAG, "onconnectionFailed ");
		
	}
	
}