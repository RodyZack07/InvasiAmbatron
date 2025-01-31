using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using Firebase.Firestore;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDB;

    public Text nameText;
    public SpriteRenderer artworkSprite;
    private FirebaseFirestore db;
    

    private int selectedOptions = 0; 
    void Start()
    {

        if (!PlayerPrefs.HasKey("selectedOptions"))
        {

            selectedOptions = 0;

        }
        else {

            Load();
        
        }

        UpdateCharacter(selectedOptions);
        
    }

    public void NextOption() {
        selectedOptions++;

        if (selectedOptions >= characterDB.characterCount) {

            selectedOptions = 0;
        }

        UpdateCharacter(selectedOptions);
    }

    public void BackOption() {
        selectedOptions--;

        if (selectedOptions <= 0) {

            selectedOptions = characterDB.characterCount - 1;
        
        }

        UpdateCharacter(selectedOptions);

    }

    private void UpdateCharacter(int selectedOptions) {

        Character character = characterDB.GetCharacter(selectedOptions);
        artworkSprite.sprite = character.characterSprite;
        nameText.text = character.characterName;
    
    }

    private void Load() {

        selectedOptions = PlayerPrefs.GetInt("selectedOptions");

    }

    private void Save() {

        PlayerPrefs.SetInt("selectedOptions", selectedOptions);

    }

    public void ChangeScene(int sceneID) {

        SceneManager.LoadScene(sceneID);
    
    }

    
}
