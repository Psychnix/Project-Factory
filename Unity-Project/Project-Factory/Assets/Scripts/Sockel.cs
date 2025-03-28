﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Sockel : SockelAbstract
{
    [SerializeField] GameObject Lichtzone;
    public bool hasLightBulbFromBeginning = false;
    
    private new void Start()
    {
        base.Start();
        _name = "Sockel";
        _interactionName = "Glühbirne einschrauben";
        allSockets.Add(this);
        Gluehbirne.SetActive(hasLightBulbFromBeginning);
        currentlyInteractable = !hasLightBulbFromBeginning;
        Lichtzone.SetActive(Gluehbirne.activeSelf && StromAktiviert);
    }

    public new void Update()
    {
      
        if (Gluehbirne.activeSelf)
        {
            Gluehbirne.GetComponentInChildren<Light>().enabled = StromAktiviert;
            Lichtzone.SetActive(StromAktiviert);
        }
    }

    public override void Interact()
    {
            In(typeof(Gluehbirne));
    }
}