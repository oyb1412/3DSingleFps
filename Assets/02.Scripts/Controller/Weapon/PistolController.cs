public class PistolController : WeaponController
{
    protected override void Awake() {
        base.Awake();
        BoundValue = 1f;
    }
    public override void Shot() {
        base.Shot();
    }
}
