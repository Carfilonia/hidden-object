using UnityEngine;

[CreateAssetMenu(fileName = "GameManagerData", menuName = "ScriptableObjects/GameManagerData")]
public class GameManagerScriptableObject : ScriptableObject
{
    //Tempo de exibi��o de load de jogo em segundos 
    public float tempoMinimoLoadSegundos = 1f;

    //Tempo de exibi��o de mensagem de jogo em segundos 
    public float showGameMessageSeconds = 2.3f;

}
