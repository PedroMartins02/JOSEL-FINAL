using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameModel
{
    public abstract class Card
    {
        public string Name {get; protected set;}
        public string ShortText { get; protected set; }
        public string Description { get; protected set; }
        public int Blessings { get; protected set; }
        //public GUID Art; // TODO: Verificar como guardar uma referencia para o asset
        public Factions Faction { get; protected set; }
        public List<Effect> Effects { get; protected set; }
        public List<Modifier> Modifiers { get; protected set; }
    }
}