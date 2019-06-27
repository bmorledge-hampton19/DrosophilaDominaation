﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlySelectorManager : MonoBehaviour
{

    public GameObject flySelector;

    public MarkerManager markerManager;
    public DropdownManager dropdownManager;
    public FlyReadoutManager flyReadoutManager;
    public SelectionManager selectionManager;
    public Text flyDestination;

    public delegate void fliesSelected(List<Fly> selectedFlies);
    public event fliesSelected sendFlies;
    public Storage flyStorage;
    private List<Fly> fliesInView;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateFliesInView() {
        
        fliesInView.Clear();
        fliesInView=flyStorage.getFlies(dropdownManager.getSelectedTraits());

        if (dropdownManager.getSelectedSex() != 0){
            if (dropdownManager.getSelectedSex() == 1) removeFliesBasedOnSex(true);
            else removeFliesBasedOnSex(false);
        }

        if (dropdownManager.getSelectedHybridization() != 0) {
            removeFliesBasedOnHybridization(dropdownManager.getSelectedHybridization());
        }

        if (markerManager.getSelectedMarkers().Count > 0){
            removeFliesBasedOnMarkers(markerManager.getSelectedMarkers());
        }

        flyReadoutManager.updateReadout(fliesInView);

    }

    private void removeFliesBasedOnSex(bool removeMale){
        List<Fly> fliesToRemove = new List<Fly>();
        foreach(Fly fly in fliesInView) {
            if (removeMale && fly.ismale()) fliesToRemove.Add(fly);
            else if (!removeMale && !fly.ismale()) fliesToRemove.Add(fly);
        }
        foreach(Fly fly in fliesToRemove) fliesInView.Remove(fly);
    }
    private void removeFliesBasedOnHybridization(int selection){
        // 1 = wild type, 2 = 1 trait, 3 = 2 traits, 4 = 1 or 2 traits, 5 = mutts

        List<Fly> fliesToRemove = new List<Fly>();
        foreach(Fly fly in fliesInView) {
            
            switch(selection){
            case 1:
                if (fly.getNumExpressedTraits() != 0) fliesToRemove.Add(fly);
                break;
            case 2:
                if (fly.getNumExpressedTraits() != 1) fliesToRemove.Add(fly);
                break;
            case 3:
                if (fly.getNumExpressedTraits() != 2) fliesToRemove.Add(fly);
                break;
            case 4:
                if (fly.getNumExpressedTraits() != 1 && fly.getNumExpressedTraits() != 2) fliesToRemove.Add(fly);
                break;
            case 5:
                if (fly.getNumExpressedTraits() < 3) fliesToRemove.Add(fly);
                break;
            }

        }
        foreach(Fly fly in fliesToRemove) fliesInView.Remove(fly);

    }
    private void removeFliesBasedOnMarkers(List<Fly.Markers> markers){
        List<Fly> fliesToRemove = new List<Fly>();
        foreach(Fly fly in fliesInView) {
            if (!fly.containsMarkers(markers)) fliesToRemove.Add(fly);
        }
        foreach(Fly fly in fliesToRemove) fliesInView.Remove(fly);
    }

    public void setUpSelector(string flyDestination, int minFlies, int maxFlies){

        this.flyDestination.text = "Destination: " + flyDestination;

        dropdownManager.setUpDropdowns();

        selectionManager.updateMinMax(minFlies,maxFlies);
        selectionManager.clearSelectedFlies();

        updateFliesInView();

        flySelector.SetActive(true);
    }
    public void selectionFinished(){
        flyReadoutManager.deleteReadouts(selectionManager.getSelectedFlies());
        flySelector.SetActive(false);
        sendFlies(selectionManager.getSelectedFlies());
    }

    public void cancelSelection(){
        flyReadoutManager.toggleAll(selectionManager.getSelectedFlies());
        selectionManager.clearSelectedFlies();
        selectionFinished();
    }

}
