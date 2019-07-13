﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JarManager : MonoBehaviour {

	public Storage storage;
	public TraitDB traitDB;
	private Jar jar;
	private int ID;
	public static int jarNum = 0;
	public void SetID(int ID){this.ID = ID;}
	
	public GameObject mainPanel;
	public JarUIManager jarUIManager;
	public ProgressBar progressBar;
	public FlySelectorManager flySelectorManager;
	public AddFliesManager addFliesManager;
	public DialoguePopup dialoguePopup;

	// Use this for initialization
	void Start () {
		
		jar = new Jar(traitDB);

		jarNum++;
		ID = jarNum;

	}

	public void instatiateUIConnections(){

		jarUIManager.setUpProgressBar(progressBar);
		progressBar.fillActions += breedFlies;

		jarUIManager.jarActionButton.pressActions += jarAction;

	}

	public void breedFlies(){
		jar.generateProgeny();
	}

	private void jarAction(JarActionButton.ButtonState buttonState){
		switch ((int)buttonState){
            case 1:
                checkOnBreeders();
                break;
            case 2:
                selectNewParents();
                break;
            case 3:
                beginBreeding();
                break;
        }
	}

	private void checkOnBreeders(){
		if(progressBar.finished()) openAddFlies();
		else {
			string dialogueText = "The flies are not finished breeding yet.  Are you sure you want to discard the parents without generating progeny?";
			dialoguePopup.setup(dialogueText,delegate{terminateJarEarly();});
		}
	}

	private void terminateJarEarly(){

		jar.emptyJar();
		jarUIManager.jarActionButton.advanceState();
		progressBar.deactivate();

	}

	private void openAddFlies() {
		addFliesManager.addFlies += emptyJar;
		addFliesManager.setUpAdder(jar.getProgeny(),jar.getParents());
		mainPanel.SetActive(false);
	}

	private void emptyJar(){
		mainPanel.SetActive(true);
		storage.addFlies(jar.emptyJar());
		addFliesManager.addFlies -= emptyJar;
		jarUIManager.jarActionButton.advanceState();
	}
	public void selectNewParents(){
		flySelectorManager.setUpSelector(("Jar " + ID), 2, 8);
		flySelectorManager.sendFlies += addNewParents;
		mainPanel.SetActive(false);
	}
	public void beginBreeding(){
		progressBar.activate(.005f);
		jarUIManager.jarActionButton.advanceState();
	}

	private void addNewParents(List<Fly> newParents) {
		mainPanel.SetActive(true);
		if (newParents.Count != 0) {
			jar.addParents(newParents);
			jarUIManager.jarActionButton.advanceState();
			storage.removeFlies(newParents);
		}
		flySelectorManager.sendFlies -= addNewParents;
	}

}
