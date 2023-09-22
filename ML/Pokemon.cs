namespace ML
{
    public class Pokemon
    {
        public int id { get; set; }
        public string Name { get; set; }
        public int weight { get; set; }
        public int height { get; set; }
        public Sprites sprites { get; set; }
        public Stat stats { get; set; }
        public Typee types { get; set; }
        public string url { get; set; }
        public string base_experience { get; set; }
        public string ability { get; set; }
        public List<object> Pokemons { get; set; }
        public Habilidad Habilidad { get; set; }
    }
}