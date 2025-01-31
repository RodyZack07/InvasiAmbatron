using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[CreateAssetMenu]
public class CharacterDatabase : ScriptableObject
{
    public Character[] character;

    public int characterCount { 
        get {
          return  character.Length;
        }
    }

    public Character GetCharacter(int index) {

        return character[index];
    
    }
}
