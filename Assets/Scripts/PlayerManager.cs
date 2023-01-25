using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Stat")]
    public float speedmovement;
    public float speedrotate;
    [SerializeField] private int dmgAttack;
    [SerializeField] private float maxHp;
    [SerializeField] private float currentHp;
    [SerializeField] private float maxMp;
    [SerializeField] private float currentMp;
    [SerializeField] private float maxStamina;
    [SerializeField] private float currentStamina;
    [SerializeField] private bool IsLife = true;
    [SerializeField] private bool CDSkill1 = false;
    [SerializeField] private bool CDSkill2 = false;
    [SerializeField] private bool CDSkill3 = false;
    private bool isSkill2 = false;
    [SerializeField] private float gainExp;
    [SerializeField] private float gainMoney;
    [SerializeField] private float maxExp;

    [SerializeField] private int currentdmg;

    private float cooldownTimerSkill1;
    private float cooldownTimerSkill2;
    private float cooldownTimerSkill3;

    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private Animator charAnimator;
    [SerializeField] private Rigidbody rb;

    [Header("Objects")]
    //private GameObject myCharacter;
    [SerializeField] private Transform skeleton;
    [SerializeField] private SkeletonManager s;
    public GameObject sword;


    [Header("Button")]
    [SerializeField] private Button atkButton;
    [SerializeField] private Button dodgeButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button skill1;
    [SerializeField] private Button skill2;
    [SerializeField] private Button skill3;

    [Header("UI")]
    [SerializeField] private Image fillHpImage;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillMpImage;
    [SerializeField] private Slider mpSlider;
    [SerializeField] private Image fillStaminaImage;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image panelSkill1;
    [SerializeField] private Text cdSkill1;
    [SerializeField] private Image panelSkill2;
    [SerializeField] private Text cdSkill2;
    [SerializeField] private Image panelSkill3;
    [SerializeField] private Text cdSkill3;
    [SerializeField] private Image fillExpImage;
    [SerializeField] private Slider expSlider;
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private GameObject panelGameVictory;
    [SerializeField] private Text expValue;
    [SerializeField] private Text text_maxexp;
    [SerializeField] private Text moneyValue;

    [SerializeField] private Transform cameraTransfrom;

    void Start()
    {
        IsLife = true;
        currentHp = maxHp;
        currentMp = maxMp;
        currentStamina = maxStamina;
        rb = GetComponent<Rigidbody>();
        atkButton.onClick.AddListener(Attack);
        jumpButton.onClick.AddListener(Jump);
        dodgeButton.onClick.AddListener(Dodge);
        skill1.onClick.AddListener(SkillOne);
        skill2.onClick.AddListener(SkillTwo);
        skill3.onClick.AddListener(SkillThree);
    }

    private void FixedUpdate()
    {
        if (IsLife)
        {
            Move();
            HPbar();
            Staminabar();
        }
        else
        {
            panelGameOver.SetActive(true);
            charAnimator.SetBool("IsLife", false);
        }
        if (currentMp < 100)
        {
            currentMp += Time.deltaTime;
            MPbar();
        }
        if (currentStamina < 100)
        {
            currentStamina += Time.deltaTime;
            Staminabar();
        }

        if (CDSkill1)
        {
            CooldownSkill1();
        }

        if (CDSkill2)
        {
            CooldownSkill2();
        }

        if (CDSkill3)
        {
            CooldownSkill3();
        }

        if (isSkill2 == true)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

    }

    private void Move()
    {
        rb.velocity = new Vector3(joystick.Horizontal * speedmovement, rb.velocity.y, joystick.Vertical * speedmovement);

        if (joystick.Vertical != 0 || joystick.Horizontal != 0)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            transform.rotation = Quaternion.LookRotation(rb.velocity);
            charAnimator.SetBool("IsWalk", true);
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            charAnimator.SetBool("IsWalk", false);
        }

        charAnimator.SetBool("IsAttack", false);
        charAnimator.SetBool("IsDodge", false);
        charAnimator.SetBool("IsJump", false);
        charAnimator.SetBool("Skill1", false);
        charAnimator.SetBool("Skill2", false);
        charAnimator.SetBool("Skill3", false);
        charAnimator.SetBool("GetHit", false);
    }

    private void Attack()
    {
        sword.gameObject.GetComponent<MeshCollider>().enabled = true;
        currentdmg = dmgAttack;
        RotateToTarget();
        charAnimator.SetBool("IsAttack", true);
        //Debug.Log("attack !");
    }

    private void Jump()
    {
        float jumpStamina = 10;
        if (currentStamina >= jumpStamina)
        {
            currentStamina -= jumpStamina;
            charAnimator.SetBool("IsJump", true);
            Staminabar();
        }
    }

    private void Dodge()
    {
        float dodgeStamina = 20;
        if (currentStamina >= dodgeStamina)
        {
            currentStamina -= dodgeStamina;
            charAnimator.SetBool("IsDodge", true);
            Staminabar();
        }
    }

    private void SkillOne()
    {
        float SkillOneMP = 5;
        int SkillOneDMG = 10;
        float cooldown = 3;

        if (currentMp >= SkillOneMP && !CDSkill1)
        {
            sword.gameObject.GetComponent<MeshCollider>().enabled = true;
            CDSkill1 = true;
            panelSkill1.gameObject.SetActive(true);
            currentdmg = SkillOneDMG;
            cooldownTimerSkill1 = cooldown;
            currentMp -= SkillOneMP;
            RotateToTarget();
            charAnimator.SetBool("Skill1", true);
            MPbar();
        }
    }

    private void SkillTwo()
    {
        float SkillTwoMP = 13;
        int SkillTwoDMG = 20;
        int cooldown = 5;
        if (currentMp >= SkillTwoMP && !CDSkill2)
        {
            sword.gameObject.GetComponent<MeshCollider>().enabled = true;
            CDSkill2 = true;
            isSkill2 = true;
            panelSkill2.gameObject.SetActive(true);
            currentdmg = SkillTwoDMG;
            cooldownTimerSkill2 = cooldown;
            currentMp -= SkillTwoMP;
            RotateToTarget();
            charAnimator.SetBool("Skill2", true);
            MPbar();
        }
    }

    private void SkillThree()
    {
        float SkillThreeMP = 16;
        int SkillThreeDMG = 30;
        int cooldown = 7;
        if (currentMp >= SkillThreeMP && !CDSkill3)
        {
            sword.gameObject.GetComponent<MeshCollider>().enabled = true;
            CDSkill3 = true;
            panelSkill3.gameObject.SetActive(true);
            currentdmg = SkillThreeDMG;
            cooldownTimerSkill3 = cooldown;
            currentMp -= SkillThreeMP;
            RotateToTarget();
            charAnimator.SetBool("Skill3", true);
            MPbar();
        }
    }

    private void HPbar()
    {
        if (hpSlider.value != 0)
        {
            float FillHpValue = currentHp / maxHp;
            hpSlider.value = FillHpValue;
        }
    }

    private void MPbar()
    {
        float FillMpValue = currentMp / maxMp;
        mpSlider.value = FillMpValue;
    }

    private void Staminabar()
    {
        float FillStaminaValue = currentStamina / maxStamina;
        staminaSlider.value = FillStaminaValue;
    }

    private void Dead()
    {
        //Debug.Log("Dead !");
        IsLife = false;
    }

    private void CooldownSkill1()
    {
        cooldownTimerSkill1 -= Time.deltaTime;
        if (cooldownTimerSkill1 < 0.0f)
        {
            CDSkill1 = false;
            panelSkill1.gameObject.SetActive(false);
        }
        else
        {
            cdSkill1.text = Mathf.RoundToInt(cooldownTimerSkill1).ToString();

        }
    }

    private void CooldownSkill2()
    {
        cooldownTimerSkill2 -= Time.deltaTime;
        if (cooldownTimerSkill2 < 0.0f)
        {
            CDSkill2 = false;
            isSkill2 = false;
            panelSkill2.gameObject.SetActive(false);
        }
        else
        {
            cdSkill2.text = Mathf.RoundToInt(cooldownTimerSkill2).ToString();
        }
    }

    private void CooldownSkill3()
    {
        cooldownTimerSkill3 -= Time.deltaTime;
        if (cooldownTimerSkill3 < 0.0f)
        {
            CDSkill3 = false;
            panelSkill3.gameObject.SetActive(false);
        }
        else
        {
            cdSkill3.text = Mathf.RoundToInt(cooldownTimerSkill3).ToString();
        }
    }

    private void RotateToTarget()
    {

        Vector3 direction = skeleton.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = rotation;
    }

    public void PlayerTakeDMG(int dmg)
    {
        currentHp -= dmg;
        s.axeLeft.GetComponent<MeshCollider>().enabled = false;
        s.axeRight.GetComponent<MeshCollider>().enabled = false;
        if (currentHp <= 0)
        {
            Dead();
        }
    }

    public int Damage()
    {
        return currentdmg;
    }

    public void GainEXPandMoney(float exp, float money)
    {
        gainExp += exp;
        gainMoney += money;

        panelGameVictory.SetActive(true);
        float FillExpValue = gainExp / maxExp;
        expSlider.value = FillExpValue;
        expValue.text = gainExp.ToString();
        text_maxexp.text = maxExp.ToString();
        moneyValue.text = gainMoney.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Axe")
        {
            PlayerTakeDMG(s.Damage());
            charAnimator.SetBool("GetHit", true);
            //Debug.Log("getDmg" + s.GetDamage());
        }
    }
}
