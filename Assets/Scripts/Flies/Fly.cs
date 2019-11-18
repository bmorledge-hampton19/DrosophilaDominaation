﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fly {

    private TraitDB traitDB;
    protected Dictionary<TraitData.TraitID, Trait> traits;
    private Dictionary<TraitData.PhenotypeGroupID, TraitData> expressedTraits;
    public List<TraitData> getExpressedTraits() {
        List <TraitData> traitsToReturn = new List<TraitData>();
        for (int i = 0; i<9; i++) {
            if (expressedTraits.ContainsKey((TraitData.PhenotypeGroupID)i)){
                traitsToReturn.Add(expressedTraits[(TraitData.PhenotypeGroupID)i]);
            }
        }
        return traitsToReturn;
    }
    public int getNumExpressedTraits()=>expressedTraits.Values.Count;
    private List<Markers> markers;

    public FlyStats stats;
    private float fitness = 1;
    public float getFitness() => fitness;
    public void setFitness(float fitness) {this.fitness = fitness;}

    private bool isMale;
    public bool ismale() => isMale;

    public enum Markers {
        red = 1,
        green = 2,
        blue = 3,
        yellow = 4,
        purple = 5,
        silver = 6,
        gold = 7
    }

    public Fly(Fly maleParent, Fly femaleParent, TraitDB traitDB) {

        this.traitDB = traitDB;

        init();

        //Initialize Traits from parental cross.
        cross(maleParent, femaleParent);

        setStats();

    }

    public Fly(TraitDB traitDB) {

        this.traitDB = traitDB;

        init();

        foreach (TraitData.TraitID key in traits.Keys.ToList()) {
            if (traitDB.getDiscoveredObjects().Contains(traits[key].getTraitData())) {           
                traits[key].setAlleles(Random.Range(0,3));
            }
            else traits[key].setAlleles(0);
            if (traits[key].isPresent(isMale)){
                if (!expressedTraits.ContainsKey(traits[key].getTraitData().PGID) || 
                expressedTraits[traits[key].getTraitData().PGID].priority > traits[key].getTraitData().priority) {
                    expressedTraits[traits[key].getTraitData().PGID] = traits[key].getTraitData();
                }
            }
        }

        setStats();

    }

    private void init() {

        traits = new Dictionary<TraitData.TraitID, Trait>();
        expressedTraits = new Dictionary<TraitData.PhenotypeGroupID, TraitData>();
        markers = new List<Markers>();

        //Roll those dice!  Boy or girl?
        isMale = UnityEngine.Random.Range(0,2) == 1;

        //Determine which traits are possible.
        foreach (TraitData traitData in traitDB.getCurrentObjectTier()) {    
            traits.Add(traitData.TID, new Trait(traitData));
        }

    }

    private void setStats() {

        stats = new FlyStats(traitDB.gameManager.getGamePhase());

        if (expressedTraits.Count > 3) {
            foreach (FlyStats.StatID stat in stats.getGamePhaseStats()) {
                stats.setStat(stat,1);
            }
        }
        else if (expressedTraits.Count != 0) {

            foreach (FlyStats.StatID stat in stats.getGamePhaseStats()) {
                stats.setStat(stat,0);
            }

            foreach(TraitData trait in expressedTraits.Values.ToList()){
                foreach (SingleStat statMod in trait.stats) {
                    stats.setStat(statMod.stat,stats.getStat(statMod.stat)+statMod.value);
                }
            }

            float statFactor;
            if (expressedTraits.Count == 1) {
                statFactor = 1;
            } else {
                statFactor = (5-expressedTraits.Count)/(6-expressedTraits.Count);
            }

            foreach (FlyStats.StatID stat in stats.getGamePhaseStats()) {
                stats.setStat(stat,(int)(stats.getStat(stat)*statFactor));
            }

            foreach (FlyStats.StatID stat in stats.getGamePhaseStats()) {
                stats.setStat(stat,stats.getStat(stat)+2);
            }

        } else {
            foreach (FlyStats.StatID stat in stats.getGamePhaseStats()) {
                stats.setStat(stat,2);
            }
        }

        foreach (TraitData trait in expressedTraits.Values.ToList()) {
            fitness *= trait.fitnessModifier;
        }

    }

    private void cross(Fly maleParent, Fly femaleParent) {
        foreach (TraitData.TraitID key in traits.Keys.ToList()) {
            if (traits[key].getTraitData().isIntermediate() 
            && traits[traits[key].getTraitData().complimentaryTraitID].getAlleles() < 3) {
                traits[key].setAlleles(traits[traits[key].getTraitData().complimentaryTraitID].getAlleles());
            } else {
                int alleles = maleParent.traits[key].getGameteAlleles(true,!isMale)
                + femaleParent.traits[key].getGameteAlleles(false,!isMale);
            
                traits[key].setAlleles(alleles);
            }
            if (traits[key].isPresent(isMale)){
                if (!expressedTraits.ContainsKey(traits[key].getTraitData().PGID) || 
                expressedTraits[traits[key].getTraitData().PGID].priority > traits[key].getTraitData().priority) {
                    expressedTraits[traits[key].getTraitData().PGID] = traits[key].getTraitData();
                }
            }
        }
    }

    public void addMarker(Markers marker){
        if (!markers.Contains(marker)){
            markers.Add(marker);
        }
    }
    public void removeMarker(Markers marker){
        if (markers.Contains(marker)){
            markers.Remove(marker);
        }
    }
    public List<Markers> getMarkers()=>markers;

    public bool hasSameTraits(List<TraitData> traits) => 
    traits.SequenceEqual(expressedTraits.Values.ToList());

    public bool containsMarkers(List<Markers> markers) =>
    !markers.Except(this.markers).Any();

    public bool containsTraits(List<TraitData> traits) =>
    !traits.Except(expressedTraits.Values.ToList()).Any();
}
