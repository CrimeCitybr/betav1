﻿//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2025 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Changes HUD image colors by UI Sliders.
/// </summary>
public class RCC_DashboardColors : RCC_Core {

    public Image[] huds;
    public Color hudColor = Color.white;

    public Slider hudColor_R;
    public Slider hudColor_G;
    public Slider hudColor_B;

    private void Start() {

        if (huds == null || huds.Length < 1)
            enabled = false;

        if (hudColor_R && hudColor_G && hudColor_B) {

            hudColor_R.value = hudColor.r;
            hudColor_G.value = hudColor.g;
            hudColor_B.value = hudColor.b;

        }

    }

    private void Update() {

        if (hudColor_R && hudColor_G && hudColor_B)
            hudColor = new Color(hudColor_R.value, hudColor_G.value, hudColor_B.value);

        for (int i = 0; i < huds.Length; i++) {

            if (huds[i] != null)
                huds[i].color = new Color(hudColor.r, hudColor.g, hudColor.b, huds[i].color.a);

        }

    }

}
