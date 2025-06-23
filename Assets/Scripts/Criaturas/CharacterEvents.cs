using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CharacterEvents
{
    // character damaged and damage value
    public static UnityEvent<GameObject, int> characterDamaged = new UnityEvent<GameObject, int>();

    // character healed and amount healed  
    public static UnityEvent<GameObject, int> characterHealed = new UnityEvent<GameObject, int>();
}