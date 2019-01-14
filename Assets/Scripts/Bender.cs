using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public abstract class Bender : MonoBehaviour
{
    public enum States
    {
        Idle,
        Moving,
        Casting1,
        Casting2,
        Casting3,
        Recovering,
        Frozen
    }

    public enum BenderTypes
    {
        Air,
        Earth,
        Fire,
        Water
    }

    public BenderTypes BenderType;

    public string Name;
    public States State = States.Idle;

    public float StandardSpeed = 5f;
    public float StandardRotationSpeed = 120f;
    public int AbilityNumber = 3;

    public Transform ProjectileSpawnPoint;

    public List<ParticleSystem> AbilitiesPS;

    public TextMeshPro Nameplate;
    public GameObject MeshChild;
    public Material OutlinedMaterial;
    public Material FrozenMaterial;
    public GameObject Base;


    public List<AudioClip> Footsteps;
    public AudioClip FallSound;
    public AudioClip AbilitySound;
    public Sprite PortraitSprite;

    [Tooltip("Check that it is equal to the animation lenght!")]
    public int RecoveringDuration = 55;

    [HideInInspector]
    public Animator BenderAnimator;
    [HideInInspector]
    public float SpeedInput = 0f;
    [HideInInspector]
    public float RotationSpeedInput = 0f;
    [HideInInspector]
    public int AbilitySelector = -1;

    [HideInInspector]
    public NavMeshAgent NavAgent;

    [HideInInspector]
    private PlayerController _owner;

    protected bool _selected = false;

    protected Rigidbody _rigidbody;
    protected Renderer _renderer;
    protected Material _defaultMaterial;
    protected AudioSource _audioSource;
    protected BenderIconController _iconObject;

    protected float _rotation;

    protected float _vulnerability;

    protected Material _prevMaterial;

    protected States _frozenPrevState = States.Idle;

    protected bool _isHit = false;
    protected bool _isDefeated = false;

    private int _recoveringCounter = 0;

    public BenderIconController IconObject
    {
        get
        {
            return _iconObject;
        }

        set
        {
            _iconObject = value;
        }
    }

    public PlayerController Owner
    {
        get
        {
            return _owner;
        }

        set
        {
            _owner = value;

            try
            {
                Color baseColor = _owner.PlayerColor;
                baseColor.a = 0.5f;
                Base.GetComponent<Renderer>().material.color = baseColor;
            }
            catch
            {

            }
        }
    }

    public AudioSource AudioSourcePlayer
    {
        get
        {
            return _audioSource;
        }
    }

    protected void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        BenderAnimator = GetComponent<Animator>();
        Name = Names.NamesList[BenderType][Random.Range(0, Names.NamesList[BenderType].Count)];
        Nameplate.text = Name;
        _renderer = MeshChild.GetComponent<Renderer>();
        _defaultMaterial = _renderer.materials[0];
        _audioSource = GetComponent<AudioSource>();
        foreach (ParticleSystem ps in AbilitiesPS)
        {
            ps.Stop();
        }
    }

    protected virtual void Start()
    {
        _vulnerability = 1.0f;
        _rotation = transform.rotation.eulerAngles.y;

    }


    protected void Update()
    {
        Debug.Log(Name + ": [State]: " + State);
        if (State != States.Idle && State != States.Moving)
        {
            // Recovering should be regulated by the duration of the Death animation (or by triggering Revive trigger 
            // by waterbender revive ability)

            if (State == States.Recovering)
            {
                if (_recoveringCounter > RecoveringDuration)
                {
                    _recoveringCounter = 0;
                    State = States.Idle;
                }
                else
                {
                    _recoveringCounter += 1;
                    return;
                }
            }
            return;
        }

        if (Owner == null)
            return;

        if (Owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            if (_selected)
            {
                if (Input.GetButtonDown("Ability1"))
                {
                    StartAbility(States.Casting1, 0);
                    return;
                }
                else if (Input.GetButtonDown("Ability2"))
                {
                    StartAbility(States.Casting2, 1);
                    return;
                }
                else if (AbilityNumber >= 3 && Input.GetButtonDown("Ability3"))
                {
                    StartAbility(States.Casting3, 2);
                    return;
                }
            }
        }
        else
        {
            switch (AbilitySelector)
            {
                case 0:
                    StartAbility(States.Casting1, 0);
                    return;
                case 1:
                    StartAbility(States.Casting2, 1);
                    return;
                case 2:
                    StartAbility(States.Casting3, 2);
                    return;
                default:
                    break;
            }
            AbilitySelector = -1;

            _rotation += RotationSpeedInput * StandardRotationSpeed * Time.deltaTime;
            _rigidbody.rotation = (Quaternion.Euler(new Vector3(0f, _rotation, 0f)));
            if (_rigidbody.velocity.magnitude <= 0.01 && _rigidbody.velocity.y <= Mathf.Epsilon)
            {
                _rigidbody.velocity = new Vector3();
            }
            _rigidbody.velocity = transform.forward * SpeedInput * StandardSpeed;
        }

        if (((Owner.Type == PlayerController.PlayerTypes.AI || Owner.Type == PlayerController.PlayerTypes.HumanKeyBoard) && Mathf.Abs(SpeedInput) > 0.01) ||
            ((Owner.Type == PlayerController.PlayerTypes.HumanMouse) && NavAgent.velocity.magnitude > 0.01))
        {
            BenderAnimator.SetBool("Moving", true);
            State = States.Moving;
            _isHit = false;
        }
        else
        {
            BenderAnimator.SetBool("Moving", false);
            State = States.Idle;
            _isHit = false;
        }
    }

    public void StartAbility(States state, int number)
    {
        if (_owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            NavAgent.isStopped = true;
        }
        BenderAnimator.SetBool("Ability", true);
        State = state;
        AbilitiesPS[number].Play();
        _rigidbody.velocity = new Vector3();
        AbilitySelector = -1;
    }

    protected abstract void Cast();

    /*public void ApplyForce(Vector3 force)
    {
        Debug.Log("Applying air force: " + force);
        _rigidbody.AddForce(force);
    }*/

    public void GotHit()
    {
        _vulnerability += 0.05f;
        BenderAnimator.SetBool("Ability", false);
        if (State != States.Frozen)
        {
            BenderAnimator.SetTrigger("Hit");
            State = States.Recovering;
            BenderAnimator.SetBool("Moving", false);
            if (_owner.Type == PlayerController.PlayerTypes.HumanMouse)
            {
                if (NavAgent.isActiveAndEnabled)
                {
                    NavAgent.isStopped = true;
                }
            }
            Debug.Log(Name + ": Got hit!");
        }
        else
        {
            BenderAnimator.SetTrigger("Hit");
            BenderAnimator.SetBool("Moving", false);
            _frozenPrevState = States.Recovering;
        }

        _isHit = true;
    }

    public bool IsHit()
    {
        /*try
        {
            return !BenderAnimator.GetBool("Moving");
        }
        catch
        {
            return false;
        }*/
        return _isHit;
    }

    public bool IsDefeated()
    {
        return _isDefeated;
    }

    public void GotRevived()
    {
        BenderAnimator.SetTrigger("Revive");
    }

    public void Recovered()
    {
        _recoveringCounter = 0;
        State = States.Idle;
        if (_owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            if (NavAgent.isActiveAndEnabled)
            {
                NavAgent.isStopped = false;
            }
        }
    }

    public void GotSelected()
    {
        _renderer.material = OutlinedMaterial;
        IconObject.SelectionBox.SetActive(true);
        _selected = true;
    }

    public void GotDeselected()
    {
        _renderer.material = _defaultMaterial;
        IconObject.SelectionBox.SetActive(false);
        _selected = false;
    }

    public void FinishCast()
    {
        Debug.Log(Name + ": Finish casting...");
        _audioSource.PlayOneShot(AbilitySound);
        BenderAnimator.SetBool("Ability", false);
        State = States.Idle;
        if (_owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            NavAgent.isStopped = false;
        }
    }

    public void GotFrozen(float freezeDuration)
    {
        _prevMaterial = _renderer.material;
        _renderer.material = FrozenMaterial;
        BenderAnimator.speed = 0f;
        _frozenPrevState = State;
        State = States.Frozen;

        StartCoroutine(Unfreeze(freezeDuration));
    }

    protected IEnumerator Unfreeze(float freezeDuration)
    {
        yield return new WaitForSeconds(freezeDuration);
        if (State == States.Frozen)
        {
            GotUnfrozen();
        }
    }

    public void GotUnfrozen()
    {
        _renderer.material = _prevMaterial;
        BenderAnimator.speed = 1f;
        if (_frozenPrevState != States.Recovering)
        {
            BenderAnimator.SetTrigger("Reset");
            Recovered();
        }
    }

    public void FootstepSound()
    {
        if (Footsteps.Count > 0)
        {
            _audioSource.PlayOneShot(Footsteps[Random.Range(0, Footsteps.Count)]);
        }
    }

    public void PlayFallSound()
    {
        _audioSource.PlayOneShot(FallSound);
    }

    public void Defeat()
    {
        if (IconObject != null)
            IconObject.RemovedMask.SetActive(true);

        if (Owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            HumanControllerMouse hc = Owner.GetComponent<HumanControllerMouse>();
            if (hc.Selection == this)
            {
                hc.Selection = null;
            }
        }

        _isDefeated = true;

        Debug.Log(name + " defeated");
        Destroy(gameObject);
    }

    public float getVulnerability()
    {
        return _vulnerability;
    }

    public bool isSelected()
    {
        return _selected;
    }
}
