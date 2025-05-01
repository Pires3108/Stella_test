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
    public static UnityEvent<GameObject, int> characterDamaged = new UnityEvent<GameObject, int>();
    public static UnityEvent<GameObject, int> characterHealed = new UnityEvent<GameObject, int>();
}