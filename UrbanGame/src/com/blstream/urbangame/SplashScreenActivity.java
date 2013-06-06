package com.blstream.urbangame;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

import com.blstream.urbangame.example.DemoData;
import com.blstream.urbangame.session.LoginManager;

public class SplashScreenActivity extends Activity {
	private LoginManager loginManager;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		//FIXME delete mock when not needed
		DemoData mock = new DemoData(this);
		mock.insertDataIntoDatabase();
		
		loginManager = LoginManager.getInstance(SplashScreenActivity.this);
		startGamesActivity();
	}
	
	private void startGamesActivity() {
		openMyGamesIfUserLoggedIn();
	}
	
	private void startGamesListActivity() {
		startSpecificActivity(GamesListActivity.class);
	}
	
	private void openMyGamesIfUserLoggedIn() {
		if (isUserLoggedIn()) {
			startMyGamesActivity();
		}
		else {
			startGamesListActivity();
		}
	}
	
	private boolean isUserLoggedIn() {
		return loginManager.isUserLoggedIn();
	}
	
	private void startMyGamesActivity() {
		startSpecificActivity(MyGamesActivity.class);
	}
	
	private void startSpecificActivity(Class<?> cls) {
		startActivity(new Intent(SplashScreenActivity.this, cls));
	}
}