
//namespace Trinity.DbProvider
//{
//    [Obsolete("No longer used, use ((MainGridProvider)Navigation.SearchAreaProvider).AddCellWeightingObstacle()")]
//    internal sealed class SearchAreaProvider : MainGridProvider, ISearchAreaProvider
//    {
//        float[] weights = null;

//        public float[] Weights
//        {
//            get { return weights; }
//            set { weights = value; }
//        }
//        WaitTimer waitTimer = null;

//        private static HashSet<string> highWeightNames = new HashSet<string>()
//        {
//            "fence", "woodfence",
//        };

//        public new float[] GetCellWeights()
//        {
//            return base.GetCellWeights();


//            Logger.Log(LogCategory.Navigator, "Enter GetCellWeights");

//            using (new PerformanceLogger("SearchAreaProvider.GetCellWeights"))
//            {
//                try
//                {
//                    // only run every if timer is done, or if this is the first time we've run
//                    if (waitTimer == null || (waitTimer != null && waitTimer.IsFinished))
//                    {
//                        // refresh the DB weights before we alter them
//                        weights = base.GetCellWeights();

//                        // if there aren't any, then lets just skip this whole activity
//                        if (weights == null)
//                            return weights;

//                        // read the objects once from the actor manager
//                        var diaObjects = ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).Select(o =>
//                            new
//                            {
//                                Name = o.Name,
//                                ActorSnoId = o.ActorSnoId,
//                                Position = o.Position,
//                                Radius = o.CollisionSphere.Radius,
//                                Weight = 1f,
//                                Dia = o
//                            }).ToList();

//                        // custom list of objects with names we know we should be walking around
//                        var objectList =
//                            (from o in diaObjects
//                             where (highWeightNames.Any(n => o.Name.ToLower().Contains(n))) // object name match
//                             select new
//                             {
//                                 Name = o.Name,
//                                 ActorSnoId = o.ActorSnoId,
//                                 Position = o.Position,
//                                 Radius = o.Radius,
//                                 Weight = 5f,
//                                 Dia = o
//                             }).ToList();

//                        // Trinity avoidances (should allow path finding around AoE)
//                        objectList.AddRange(
//                            (from o in diaObjects
//                             where AvoidanceManager.GetAvoidanceType(o.ActorSnoId) != AvoidanceType.None
//                             select new
//                             {
//                                 Name = o.Name,
//                                 ActorSnoId = o.ActorSnoId,
//                                 Position = o.Position,
//                                 Radius = AvoidanceManager.GetAvoidanceRadiusBySNO(o.ActorSnoId, o.Radius),
//                                 Weight = 2f,
//                                 Dia = o
//                             }).ToList());

//                        foreach (var obj in objectList)
//                        {
//                            Logger.Log(LogCategory.Navigator, "Multiplying cell weight by {0} for {1} ({2}) at {3} radius={4}", obj.Weight, obj.Name, obj.ActorSnoId, obj.Position, obj.Radius);
//                            var position = obj.Position;
//                            float radius = obj.Radius;
//                            Vector3 vector = new Vector3(position.X - radius, position.Y - radius, position.Z);
//                            Vector3 vector2 = new Vector3(position.X + radius, position.Y + radius, position.Z);
//                            for (float num2 = vector.Y; num2 < vector2.Y; num2 += 2.5f)
//                            {
//                                for (float num3 = vector.X; num3 < vector2.X; num3 += 2.5f)
//                                {
//                                    Point cell = MathHelper.IngameToCell(num3, num2, base.BoundsMin, base.BoundsMax, base.Width, base.Height);

//                                    int cellIndex = cell.Y * base.Width + cell.X;
//                                    base.SearchArea[cellIndex] = false;
//                                    weights[cellIndex] *= obj.Weight;
//                                }
//                            }

//                        }
//                        // if this is our first run, set a new wait timer
//                        if (waitTimer == null)
//                        {
//                            waitTimer = new WaitTimer(TimeSpan.FromMilliseconds(750));
//                        }
//                        else // reset the wait timer
//                            waitTimer.Reset();
//                    }
//                    else // wasn't time to refresh yet, use previously generated weights
//                    {
//                        Logger.Log(LogCategory.Navigator, "Using existing weights ({0})", weights.Length);
//                    }
//                }
//                catch
//                {
//                    base.Update();
//                }
//                Logger.Log(LogCategory.Navigator, "Exit GetCellWeights");
//                return weights;
//            }
//        }
//    }
//}