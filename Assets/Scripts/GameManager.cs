using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Vari�vel est�tica (todas as inst�ncias dessa classe compartilham o mesmo valor)
    //Usada para armazenar a inst�ncia atual de GameManager
    //Permitindo acesso em outras classes apenas usando GameManager.instance
    public static GameManager instance;

    public GameManagerScriptableObject gameManagerData;

    //Tempo base de dura��o de um n�vel em segundos
    public float contadorTimerSegundos
    {
        get => levelManager ? levelManager.contadorSegundos : 90f;
        set {
            if (levelManager)
            {
                levelManager.contadorSegundos = value;
            }
        }
    }

    //Pontua��o base de um item coletado
    public float pontoBasePorItem
    {
        get => levelManager ? levelManager.pontoBasePorItem : 5000f;
    }

    //Quantidade m�xima de colet�veis que ser�o exibidos
    public int maximoColetavel
    {
        get => levelManager ? levelManager.maximoColetavel : 9;
    }

    public List<ColetavelName> listaItensColetaveis
    {
        get => levelManager ? levelManager.listaItensColetaveis : null;
    }

    //Tempo de exibi��o de load de jogo em segundos 
    public float tempoMinimoLoadSegundos
    {
        get => gameManagerData.tempoMinimoLoadSegundos;
        set => gameManagerData.tempoMinimoLoadSegundos = value;

    }

    //Tempo de exibi��o de mensagem de jogo em segundos 
    public float showGameMessageSeconds
    {
        get => gameManagerData.showGameMessageSeconds;
        set => gameManagerData.showGameMessageSeconds = value;

    }

    //Estas s�o as propriedades do GameManager
    //Propriedades funcionam como atalhos para acessar e manipular informa��es
    //Tamb�m permitem a execu��o de outros c�digos ao buscar ou alterar a propriedade
    public string nomePersonagem
    {
        //ao usar GameManager.instance.nomePersonagem � retornado o nome do personagem armazenado na estrutura de dados
        get => saveGameManager.playerData.name;
        //ao usar GameManager.instance.nomePersonagem = "Fulano" � armazenado o nome do personagem e salvo em arquivo
        set
        {
            saveGameManager.playerData.name = value;
            saveGameManager.SaveGame();
        }
    }

    public string scoreTotal
    {
        get
        {
            float scoreT = 0f;
            saveGameManager.playerData.levelDataList.ForEach(x => x.faseDataList.ForEach(y => scoreT += y.score ));
            return scoreT.ToString();
        }
    }

    public string levelAtual
    {
        get => _levelAtual;
        set
        {
            _levelAtual = value;
        }
    }

    public int faseAtual
    {
        get =>  _faseAtual;
    }

    //Propriedade jogoIniciado que define quando um jogo j� foi salvo com informa��es relevantes para continuar a gameplay
    public bool jogoIniciado
    {
        get => saveGameManager.playerData.jogoIniciado;
        set
        {
            saveGameManager.playerData.jogoIniciado = value;
            saveGameManager.SaveGame();
        }
    }

    //Propriedade de n�vel de m�sica alterado pelo painel de Op��es
    public float musicaVolume
    {
        get => saveGameManager.playerData.musicVolume;
        set
        {
            //altera a propriedade volume do Componente AudioSource do GameManager
            mAudioSource.volume = value;
            //altera o valor na estrutura de dados do SaveGame
            saveGameManager.playerData.musicVolume = value;
            //executa o m�todo SaveGame salvando o jogo em arquivo
            saveGameManager.SaveGame();
        }
    }

    //Propriedade muted que altera a exist�ncia de som no jogo
    public bool muted
    {
        get => saveGameManager.playerData.muted;
        set
        {
            //A exclama��o antes de value inverte a condi��o do bool que est� recebendo
            //Se GameManager.instance.muted = true
            //ent�o a propriedade enabled do componente de escuta recebe o oposto (false)
            mAudioListener.enabled = !value;
            //armazena o valor na estrutura de dados e salva
            saveGameManager.playerData.muted = value;
            saveGameManager.SaveGame();
        }
    }

    public string nextSceneToLoad
    {
        get => sceneNameToLoad;
    }

    //Componente do Gameobject GameManager para controle do muted
    private AudioListener mAudioListener;

    //Componente do Gameobject GameManager para controle de volume da m�sica
    private AudioSource mAudioSource;

    //Controle do GameObject GameMessage que foi pr� adicionado na scene atual
    //O GameMessage faz seu registro nesta propriedade 
    private GameMessageController gameMessage;

    //Classe respons�vel por salvar o jogo
    //O pr�prio GameManager cria um objeto dessa classe ao iniciar
    private SaveGameManager saveGameManager;

    //Gerenciador de n�veis
    //Ao iniciar uma scene onde exista um GameObject LevelManager
    //este ir� armazenar sua refer�ncia aqui
    private LevelManager levelManager;

    private string sceneNameToLoad = "MenuScene";

    private string _levelAtual = "";
    private int _faseAtual = 0;

    //Ao iniciar um GameObject adicionado na scene
    public void Start()
    {
        //se ainda n�o foi registrado nenhum GameManager
        //ent�o esse seria o primeiro a ser adicionado
        if (instance == null)
        {
            //assume o controle do jogo e incia configura��o

            //registra componente de escuta de Audio no jogo
            mAudioListener = GetComponent<AudioListener>();

            //registra componente de Musica do jogo
            mAudioSource = GetComponent<AudioSource>();

            //inicia um novo Gerenciador de SaveGame
            //o Gerenciador j� busca pelo jogo salvo ao ser iniciado
            saveGameManager = new SaveGameManager();

            //ajusta o volume do componente conforme salvo em arquivo
            mAudioSource.volume = saveGameManager.playerData.musicVolume;

            //muta ou desmuta o jogo conforme salvo em arquivo
            mAudioListener.enabled = !saveGameManager.playerData.muted;

            //armaezena esse pr�prio primeiro GameManager na vari�vel instance assumindo o controle do jogo
            instance = this;

            //Configura esse GameObject para n�o ser destru�do ao trocar de scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //S� � poss�vel existir UM GameManager no jogo
            //Caso outro GameManager seja adicionado 
            //j� vai existir outro GameManager adicionado na vari�vel instance
            //Portanto qualquer tentativa novo registro resulta em remo��o desse objeto
            Destroy(gameObject);
        }
    }

    //m�todo de registro de LevelManager que � executado no OnStart do LevelManager em cada scene
    public void setLevelManager(LevelManager novoLevelManager)
    {
        levelManager = novoLevelManager;
    }

    //m�todo para coletar um item
    //dessa forma podemos adicionar regras de jogo ao coletar qualquer item no jogo todo
    public void coletarItem(ColetavelName coletavelNome, GameObject coletavel)
    {
        if (levelManager) levelManager.coletarItem(coletavelNome, coletavel);
    }

    public bool itemEstaNaListaDeColetaveis(ColetavelName nomeColetavel)
    {
        return levelManager ? levelManager.listaItensColetaveis.Contains(nomeColetavel) : false;
    }

    public void LevelTimerUpdate(float time)
    {
        levelManager.timeLeft = time;
    }
    public void LevelTimeOut()
    {
       carregarScene("GameOverScene");
    }

    public void SaveLevel(string nome, List<ColetavelName> itensColetados, int faseAtual, float score)
    {
        LevelData levelData = saveGameManager.playerData.levelDataList.Find(x => x.name == nome);

        if (levelData == null)
        {
            levelData = new LevelData();
            levelData.name = nome;
            saveGameManager.playerData.levelDataList.Add(levelData);
        }

        //saveGameManager.playerData.itensColetados.Concat(itensColetados);

        levelData.ultimaFaseConcluida = faseAtual;
        //levelData.score += score;

        saveGameManager.SaveGame();
    }

    public float GetLevelScore()
    {
        return levelManager.score;
    }
    public void saveGame()
    {
        saveGameManager.SaveGame();
    }

    public void setGameMessageController(GameMessageController gameMessage)
    {
        this.gameMessage = gameMessage;
        this.gameMessage.displaySeconds = showGameMessageSeconds;
    }

    public void showGameMessage(string message)
    {
        if (gameMessage) gameMessage.showMessage(message);
    }

    public void carregarScene(string nomeScene)
    {
        sceneNameToLoad = nomeScene;
        SceneManager.LoadScene("LoadScene");
    }

    public void resetGame()
    {
        carregarScene("MenuScene");
    }
}
