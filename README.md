<img src="https://github.com/GuilhermeHLSouza/Atividade-Raycast/blob/main/islandia-vulcao_widelg.jpg" alt="Minha Cena" width="100%" />

# Atividade-Raycast

>[!Important]
 > ` - Projeto:`
>- Turma: 3°Mtec Desenvolvimento de Jogos Digitais.
>- Escola: Etec Prof. Basilides de Godoy.
>- Sobre: Atividade realizada em sala de aula com as duplas de laboratório, com o intuito de apresentar as habilidades aprendidas em aula.
>- Link do Google Drive: https://drive.google.com/drive/folders/1OABa5OcVCN5Xn1ZWHRXPi05XaW56zzwD 

>[!Note]
 > ` - Dupla:`
>- Felipe Mussato Rodrigues
>- Guilherme Henrique Lemes de Souza



_________________________________________________________________

## Video da cena:

https://github.com/user-attachments/assets/df62ed69-bf98-4ccf-acaa-c93b5a7989ae

_________________________________________________________________
## Explicação dos conceitos:

>- RayCast: RayCast é um raio que sai de um determinado ponto e vai até atingir algo. Na cena foi utilizado para verificar a posição que foi atingido e levar o player até ele  
>- Destroy: Destroy é utilizado para destroir um GameObject. Na cena foi utilizado para destroir o player quando ele encosta no chão de lava  
>- Prefab: Prefabs são objetos pré fabricados para ser utilizados sem ser nescessario ser feito alguma alteração. Na cena foi utilizado para criar o cenário de forma rápida e prática


_________________________________________________________________
## Explicação dos scripts:

**- MovCam** <br/>
  Um script criado para impedir que a camera saia da posição dela
```csharp
public class MoveCam : MonoBehaviour
{
    public Transform cameraPosition;

    void Update()
    {
        transform.position = cameraPosition.position; //Coloca a camera na posição expecificada anteriormente
    }
}
```
**- PlayerCam**<br/>
  Controla a rotação da camera que está fixa no personagem 
```csharp
public class PlayerCam : MonoBehaviour
{
    //Declaração de variáveis 
    public float sensX;
    public float sensY;

    float yRotation;
    float xRotation;

    public Transform orientation;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Bloqueia o cursor na tela 
        Cursor.visible = false; //O cursor fica invisivel
    }

    void Update()
    {
        //Pegando o sentido que está movimentando o mouse 
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        //puxando para variavel 
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //aplicando cena
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
```
**- Ground** <br/>
  Verifica quando o player encosta no chão 
```csharp
public class DestroyPlayerOnTouch : MonoBehaviour
{
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag)) //Verica se o player colidiu com o chão 
        {
            Destroy(other.gameObject); //Destroi o player quando ele encosta no chão com lava 
        }
    }
}
```
**- Player** <br/>
 Configura a movimentação do player
 
```csharp

public class PlayerMove : MonoBehaviour
{
    //Declaração de variáveis 
    public float moveSpeed;
    float jumpForce = 7f;
    float gravity = -20f;

    public float playerHeight = 2f;
    public LayerMask WhatIsGround;
    bool grounded;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Vector3 velocity;

    public Transform orientation;

    CharacterController characterController;
    Animator animator;

    public bool freeze; 

    private void Start()
    {
        characterController = GetComponentInChildren<CharacterController>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (freeze) return;

        //Chamando os métodos
        GroundCheck();
        MyInput();
        HandleJump();
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = UnityEngine.Input.GetAxisRaw("Horizontal");
        verticalInput = UnityEngine.Input.GetAxisRaw("Vertical");
    }

    private void GroundCheck()
    {
        Vector3 rayOrigin = characterController.bounds.center;
        float rayLength = (characterController.height / 2f) + 0.1f;
        grounded = Physics.Raycast(rayOrigin, Vector3.down, rayLength, WhatIsGround);
    }

    private void HandleJump()
    {
        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (grounded && UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        Vector3 move = moveDirection.normalized * moveSpeed;

        characterController.Move((move + new Vector3(0, velocity.y, 0)) * Time.deltaTime);

        Vector3 lookDirection = orientation.forward;
        lookDirection.y = 0f;
        lookDirection.Normalize();

        if (lookDirection != Vector3.zero)
        {
            transform.forward = lookDirection;
        }
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        StopAllCoroutines();
        StartCoroutine(JumpToPositionRoutine(targetPosition, trajectoryHeight));
    }

    private IEnumerator JumpToPositionRoutine(Vector3 target, float height)
    {
        Vector3 startPos = transform.position;
        float g = -gravity;

        Vector3 displacementXZ = new Vector3(target.x - startPos.x, 0, target.z - startPos.z);
        float displacementY = target.y - startPos.y;

        float time = Mathf.Sqrt(2 * height / g) + Mathf.Sqrt(2 * Mathf.Max(0, displacementY - height) / g);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * g * height);
        Vector3 velocityXZ = displacementXZ / time;

        float elapsedTime = 0;

        freeze = true;

        while (elapsedTime < time)
        {
            characterController.Move((velocityXZ + velocityY) * Time.deltaTime);
            velocityY += Vector3.up * gravity * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        freeze = false;
        velocity = Vector3.zero;
    }
}
```
**- Grapple** <br/>
  Adiciona a funcionalidade do Raycast onde vai puxar o player para a posição atingida com um delay que pode ser alterado
```csharp
public class Grappling : MonoBehaviour
{
    //Declaração de variáveis 
    private PlayerMove pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    public float grapplingCd;
    private float grapplingCdTimer;

    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start()
    {
        pm = GetComponent<PlayerMove>();
        lr.positionCount = 0;
        lr.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple(); //verifica se o mouse foi clicado caso seja verdadeiro ele ativa ativa o método StartGrapple 

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (!grappling) return;

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            pm.freeze = true;
            grappling = true;

            lr.positionCount = 2;
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
            lr.enabled = true;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0)
            highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        pm.freeze = false;
        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
        lr.positionCount = 0;
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}

```

_________________________________________________________________

## Contribuição da dupla:

- Felipe: Criação da cena e aplicação dos conceitos

- Guilherme: Revisão e documentação 

_________________________________________________________________
## Refêrencias:

[Skyboxes](https://assetstore.unity.com/packages/2d/textures-materials/sky/free-hdr-skyboxes-pack-175525)

[Shaders Lava](https://assetstore.unity.com/packages/vfx/shaders/free-lava-shader-292492)
