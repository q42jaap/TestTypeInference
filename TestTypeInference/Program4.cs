﻿
using System;
using System.Collections.Generic;

namespace TestTypeInference4
{

	public class Animal { }
	public class Mammal : Animal { }
	public class Giraffe : Mammal { }
	public class Reptile : Animal { }

	public abstract class ZooKeeperBase
	{
		internal string name;
	
	}

	public class ZooKeeper<T> : ZooKeeperBase
		where T : Animal
	{
		internal List<T> animalsFed = new List<T>();
	}

	public static class ZooKeeperExtensions
	{

		// this method needs to be fluent
		public static TZooKeeper Name<TZooKeeper>(this TZooKeeper zooKeeper, string name)
			where TZooKeeper : ZooKeeperBase
		{
			zooKeeper.name = name;
			return zooKeeper;
		}


		// this method needs to be fluent
		public static TZooKeeper FeedAnimal<TZooKeeper, T>(this TZooKeeper zooKeeper, T animal)
			where TZooKeeper : ZooKeeper<T>
			where T : Animal
		{
			zooKeeper.animalsFed.Add(animal);
			return zooKeeper;
		}


		// this method needs to be fluent
		public static TZooKeeper FeedAnimal<TZooKeeper, T>(this TZooKeeper zooKeeper, Func<T> animalSelector)
			where TZooKeeper : ZooKeeper<T>
			where T : Animal
		{
			zooKeeper.animalsFed.Add(animalSelector());
			return zooKeeper;
		}
	}

	public class ExperiencedZooKeeper<T> : ZooKeeper<T>
		where T : Animal
	{
		internal List<T> animalsCured = new List<T>();

		// this method needs to be fluent
		public ExperiencedZooKeeper<T> CureAnimal(T animal)
		{
			animalsFed.Add(animal);
			return this;
		}
	}


	public class Program4
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

			new ExperiencedZooKeeper<Reptile>()
				.Name("Eric")
				.FeedAnimal(() => reptile)
				.CureAnimal(reptile);

		}
	}

}
