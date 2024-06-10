using System;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

[System.Serializable]
public class Fase
{
    public string nome = "Fase";
    public List<ColetavelName> listaColetaveis;
}

[CreateAssetMenu(fileName = "LevelManagerData", menuName = "ScriptableObjects/LevelManagerData")]
public class LevelManagerScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<Fase> listaFases = new List<Fase>();

    //Tempo base de dura��o de um n�vel em segundos
    public float contadorSegundos = 90;

    //Quantidade m�xima de colet�veis que ser�o exibidos
    public int maximoColetavel;

    //Pontua��o base de um item coletado
    public float pontoBasePorItem = 5000;
}
