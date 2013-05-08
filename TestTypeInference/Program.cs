
using System.Collections.Generic;

namespace TestTypeInference
{

	public class Animal { }
	public class Mammal : Animal { }
	public class Giraffe : Mammal { }
	public class Reptile : Animal { }

	public class ZooKeeper<T>
		where T : Animal
	{
		internal string name;
		internal List<T> animalsFed = new List<T>();

		// this method needs to be fluent
		public ZooKeeper<T> Name(string name)
		{
			this.name = name;
			return this;
		}

		// this method needs to be fluent
		public ZooKeeper<T> FeedAnimal(T animal)
		{
			animalsFed.Add(animal);
			return this;
		}
	}

	public class ExperiencedZooKeeper<T> : ZooKeeper<T>
		where T : Animal
	{
		internal List<T> animalsCured = new List<T>();

		// this method needs to be fluent
		// but we must new it in order to be able to call CureAnimal after this
		public new ExperiencedZooKeeper<T> Name(string name)
		{
			base.Name(name);
			return this;
		}

		// this method needs to be fluent
		// but we must new it in order to be able to call CureAnimal after this
		public new ExperiencedZooKeeper<T> FeedAnimal(T animal)
		{
			base.FeedAnimal(animal);
			return this;
		}

		// this method needs to be fluent
		public ExperiencedZooKeeper<T> CureAnimal(T animal)
		{
			animalsCured.Add(animal);
			return this;
		}
	}


	public class Program
	{
		public static void Main(string[] args)
		{
			var giraffe = new Giraffe();
			new ZooKeeper<Giraffe>()
				.Name("Jaap")
				.FeedAnimal(giraffe);

			var reptile = new Reptile();
			new ExperiencedZooKeeper<Reptile>()
				.Name("Martijn")
				.FeedAnimal(reptile)
				.CureAnimal(reptile);
		}
	}

}
