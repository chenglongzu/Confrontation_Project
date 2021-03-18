namespace ShimmerSqlite
{
    public class PlayerEntity : DataBase
    {
        public string passward;

        public PlayerEntity()
        {
        }

        public PlayerEntity(int id, string passward)
        {
            this.id = id;
            this.passward = passward;
        }

        //输出变量的值
        public override object[] DataToArray()
        {
            return new object[] { id, passward};
        }

        //输出变量的名称
        public override string[] NameToArray()
        {
            return new string[] { "id", "passward"};
        }

        //输出变量的类型
        public override string[] TypeToArray()
        {
            return new string[] { "int", "string" };
        }
    }
}