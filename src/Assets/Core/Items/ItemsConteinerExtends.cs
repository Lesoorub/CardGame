namespace Assets.Core.Items.Containers
{
    public static class ItemsConteinerExtends
    {
        public static bool Transition(this IItemsContainer conteiner, IItemsContainer anotherContrainer, ItemStack item)
        {
            if (!anotherContrainer.CanPutItem(item))
                return false;
            var i = conteiner.RemoveItem(item);
            if (!i.HasValue) return false;
            anotherContrainer.PutItem(i.Value);
            return true;
        }
    }
}