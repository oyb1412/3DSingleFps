public class RifleController : WeaponController
{
    protected override void Awake() {
        base.Awake();
        BoundValue = 0.1f;
    }
    public override void Shot() {
        base.Shot();
    }
}
