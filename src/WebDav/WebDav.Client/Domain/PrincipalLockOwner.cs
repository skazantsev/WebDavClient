namespace WebDav
{
    public class PrincipalLockOwner : LockOwner
    {
        private readonly string _principalName;

        public PrincipalLockOwner(string principalName)
        {
            Guard.NotNull(principalName, "principalName");
            _principalName = principalName;
        }

        public override string Value
        {
            get { return _principalName; }
        }
    }
}
