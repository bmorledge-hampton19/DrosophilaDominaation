﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyReadout : MonoBehaviour
{

    public Dictionary<Fly.Markers,Image> markers;
    public Image redMarker;
    public Image greenMarker;
    public Image blueMarker;
    public Image yellowMarker;
    public Image purpleMarker;
    public Image silverMarker;
    public Image goldMarker;

    public Text sext;
    public Text traitText;
    public Toggle toggle;

    public Sprite emptyMarker;
    public Sprite filledMarker;

    public void setFly(Fly fly){

        foreach (Fly.Markers marker in fly.getMarkers()){
            markers[marker].sprite = filledMarker;
        }

        if (fly.ismale()) sext.text = "M";
        else sext.text = "F";

        string traitNames = "";
        foreach (TraitData trait in fly.getExpressedTraits()){
            traitNames += (trait.name + ", ");
        }
        if (fly.getExpressedTraits().Count == 0) traitNames = "Wild Type";
        traitText.text = traitNames;

    }

    // Start is called before the first frame update
    void Start()
    {
        markers.Add(Fly.Markers.red, redMarker);
        markers.Add(Fly.Markers.green, greenMarker);
        markers.Add(Fly.Markers.blue, blueMarker);
        markers.Add(Fly.Markers.yellow, yellowMarker);
        markers.Add(Fly.Markers.purple, purpleMarker);
        markers.Add(Fly.Markers.silver, silverMarker);
        markers.Add(Fly.Markers.gold, goldMarker);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
