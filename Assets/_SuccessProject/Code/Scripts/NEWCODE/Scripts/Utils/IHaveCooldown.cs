using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHaveCooldown
{
    int CoolDownId { get; }
    float CoolDownDuration { get; }
}
