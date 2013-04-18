package com.blstream.urbangame.database.test;

import android.test.AndroidTestCase;
import android.test.IsolatedContext;

import com.blstream.urbangame.database.Database;
import com.blstream.urbangame.database.DatabaseInterface;
import com.blstream.urbangame.database.entity.Player;

public class DatabaseUserDeleteTest extends AndroidTestCase {

	private DatabaseInterface database;
	private IsolatedContext isolatedContext;
	
	@Override
	protected void setUp() throws Exception {
		super.setUp();

		if (isolatedContext == null)
			isolatedContext = new IsolatedContext(null, mContext);
		if (database == null)
			database = new Database(isolatedContext);
	}

	@Override
	protected void tearDown() throws Exception {
		super.tearDown();
		isolatedContext.deleteDatabase(Database.DATABASE_NAME);
		database = null;
	}

	private void prepareData(String email, String pass, String displayName,
			String avatarBase64) {
		Player player = new Player(email, pass, displayName, avatarBase64);
		database.insertUser(player);
	}
	/* ------------------------ USER DELETION ------------------------ */
	public void testNothingInDatabase() {
		boolean isSuccessful = database.deletePlayer("a");
		assertFalse(isSuccessful);
	}
	
	public void testOneItemInDatabaseNoMatch() {
		prepareData("email", "pass", "displayName", "avatarBase64");
		boolean isSuccessful = database.deletePlayer("a");
		assertFalse(isSuccessful);
	}
	
	public void testOneItemInDatabaseMatch() {
		prepareData("email", "pass", "displayName", "avatarBase64");
		boolean isSuccessful = database.deletePlayer("email");
		assertTrue(isSuccessful);
	}
	
	public void testManyItemsInDatabaseNoMatch() {
		for(int i = 0; i < 30; i++) {
			prepareData("email" + i, "pass" + i, "displayName" + i, "avatarBase64" + i);
		}
		boolean isSuccessful = database.deletePlayer("a");
		assertFalse(isSuccessful);
	}
	
	public void testManyItemsInDatabaseMatch() {
		for(int i = 0; i < 30; i++) {
			prepareData("email" + i, "pass" + i, "displayName" + i, "avatarBase64" + i);
		}
		boolean isSuccessful = database.deletePlayer("email" + 13);
		assertTrue(isSuccessful);
	}
	/* ------------------------ USER DELETION END ------------------------ */
}
