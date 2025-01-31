using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
public class PlayerScript : MonoBehaviour
{
    public CharacterDatabase characterDB;

    public SpriteRenderer artworkSprite;

    private int selectedOptions = 0;
    void Start()
    {

        if (!PlayerPrefs.HasKey("selectedOptions"))
        {

            selectedOptions = 0;

        }
        else
        {

            Load();

        }

        UpdateCharacter(selectedOptions);

    }

    private void UpdateCharacter(int selectedOptions)
    {

        Character character = characterDB.GetCharacter(selectedOptions);
        artworkSprite.sprite = character.characterSprite;
      
    }

    private void Load()
    {

        selectedOptions = PlayerPrefs.GetInt("selectedOptions");

    }


}
