﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKipp : MonoBehaviour
{
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !GetComponent<Animation>().isPlaying)
        {
            PlayDoorAnim();
        }
    }
    private int m_lastIndex = 0;

    public void PlayDoorAnim()
    {
        if (!GetComponent<Animation>().isPlaying)
        {
            if (m_lastIndex == 0)
            {
                GetComponent<Animation>().Play("Kipp_Open");
                m_lastIndex = 1;
            }
            else
            {
                GetComponent<Animation>().Play("Kipp_Close");
                m_lastIndex = 0;
            }
        }
    }
}
