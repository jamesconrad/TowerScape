﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordGenerator: MonoBehaviour {
    
    public GameObject player;

    public GameObject[] HiltComponents;
    public GameObject[] GuardComponents;
    public GameObject[] BladeComponents;

    private CollisionTreeManager playerCTM;
    private BuffManager playerBM;
    private PlayerScript playerS;

    [Tooltip("Must be length of 5\nMust add up to 1\nFirst element is Common, last is Legendary")]
    public float[] RarityProbabilities;

    private GameObject sword;

    public GameObject Generate(int hiltIndex, int guardIndex, int bladeIndex )
    {
        Destroy(sword);

        Transform blade = GenerateBladeComponent(bladeIndex);
        sword = blade.gameObject;
        GenerateHiltComponent(hiltIndex, blade);
        GenerateGuardComponent(guardIndex, blade);

        

        IntakeGenerator ig = blade.GetComponent<IntakeGenerator>();
        playerCTM.weapon = ig.gameObject;
        ig.buffManager = playerBM;
        playerCTM.weapon.layer = 10;

        playerS.UpdateIG(ig);

        return blade.gameObject;
    }
    
    private void GenerateHiltComponent(int componentIndex, Transform parent)
    {
        GameObject go = Instantiate(HiltComponents[componentIndex], parent);
    }
    private void GenerateGuardComponent(int componentIndex, Transform parent)
    {
        GameObject go = Instantiate(GuardComponents[componentIndex], parent);
    }
    private Transform GenerateBladeComponent(int componentIndex)
    {
        GameObject blade = Instantiate(BladeComponents[componentIndex], transform.parent);
        blade.tag = "IntakeSource";
        blade.transform.localPosition = transform.localPosition;
        blade.transform.localRotation = transform.localRotation;
        IntakeGenerator ig = blade.AddComponent<IntakeGenerator>();
        ig.itype = Intake.IntakeType.DAMAGE;
        //can be phys, ice, fire, or chaos or pure i geuss cus we have no mitigation
        ig.iclass = Intake.IntakeClass.PHYSICAL;
        //random this, based of rarity common = 17.5 - 22.5, uncommon = 20-25, rare = 22.5-27.5, epic = 25-30, leggo = 27.5 - 32.5
        //for each line in the file
        float ranval = Random.value;
        int rarity = 0;
        for (; rarity < RarityProbabilities.Length; rarity++)
        {
            ranval -= RarityProbabilities[rarity];
            if (ranval <= 0)
                break;
        }
        ig.ammount = 17.5f + rarity * 2.5f + Random.Range(0f,5f);
        return blade.transform;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerRoot");
        playerCTM = player.GetComponent<CollisionTreeManager>();
        playerBM = player.GetComponent<BuffManager>();
        playerS = player.GetComponent<PlayerScript>();
        sword = Generate(Random.Range(0, HiltComponents.Length), Random.Range(0, GuardComponents.Length), Random.Range(0, BladeComponents.Length));
    }

}
