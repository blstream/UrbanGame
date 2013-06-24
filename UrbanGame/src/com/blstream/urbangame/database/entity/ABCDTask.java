package com.blstream.urbangame.database.entity;

import java.util.Date;

import android.os.Parcel;
import android.os.Parcelable;

public class ABCDTask extends Task {
	
	public static final String FIELD_NAME_QUESTION = "Question";
	public static final String FIELD_NAME_ANSWERS = "Answers";
	
	private String question;
	private String[] answers;
	
	public ABCDTask() {
		super();
		question = null;
		answers = null;
	}
	
	/**
	 * @param id
	 * @param title
	 * @param pictureBase64
	 * @param description
	 * @param isRepetable
	 * @param isHidden
	 * @param numberOfHidden
	 * @param endTime
	 * @param maxPoints
	 * @param question
	 * @param answers
	 */
	public ABCDTask(Long id, String title, String pictureBase64, String description, Boolean isRepetable,
		Boolean isHidden, Integer numberOfHidden, Date endTime, Integer maxPoints, String question, String[] answers) {
		super(id, Task.TASK_TYPE_ABCD, title, pictureBase64, description, isRepetable, isHidden, numberOfHidden,
			endTime, maxPoints);
		this.question = question;
		setAnswers(answers);
	}
	
	/**
	 * @return the answers
	 */
	public String[] getAnswers() {
		return answers;
	}
	
	/**
	 * @param answers the answers to set
	 */
	public void setAnswers(String[] answers) {
		this.answers = answers;
	}
	
	/**
	 * @return the question
	 */
	public String getQuestion() {
		return question;
	}
	
	/**
	 * @param question the question to set
	 */
	public void setQuestion(String question) {
		this.question = question;
	}
	
	public ABCDTask(Parcel in) {
		super(in);
		question = in.readString();
		int size = in.readInt();
		answers = new String[size];
		for (int i = 0; i < size; i++) {
			answers[i] = in.readString();
		}
	}
	
	public static final Parcelable.Creator<Task> CREATOR = new Parcelable.Creator<Task>() {
		@Override
		public Task createFromParcel(Parcel in) {
			return new ABCDTask(in);
		}
		
		@Override
		public Task[] newArray(int size) {
			return new Task[size];
		}
	};
	
	@Override
	public void writeToParcel(Parcel out, int flags) {
		super.writeToParcel(out, flags);
		out.writeString(question);
		out.writeInt(answers.length);
		for (String answer : answers) {
			out.writeString(answer);
		}
	}
}
