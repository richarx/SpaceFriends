public class Banjo : UsableItem
{
    public override void UseItem(ItemHandler itemHandler)
    {
        itemHandler.gameObject.GetComponent<PlayInstrument>().PlayBanjo();
    }
}
