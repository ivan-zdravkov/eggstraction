using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    public class LevelGenerator
    {
        private int horizontalSize;

        private int minimumPlatformLength;
        private int maximumPlatformLength;
        private int averagePlarformLength;

        private int minimumSpaceLength;
        private int maximumSpaceLength;
        private int averageSpaceLength;

        private Random randomGenerator;
        private Platform[][] level;
        
        public LevelGenerator(int horizontalSize, int minimumPlatformLength, int maximumPlatformLength, int minimumSpaceLength, int maximumSpaceLength)
        {
            this.horizontalSize = horizontalSize;

            this.minimumPlatformLength = minimumPlatformLength;
            this.maximumPlatformLength = maximumPlatformLength;
            this.averagePlarformLength = (minimumPlatformLength + maximumPlatformLength) / 2;

            this.minimumSpaceLength = minimumSpaceLength;
            this.maximumSpaceLength = maximumSpaceLength;
            this.averageSpaceLength = (minimumSpaceLength + maximumSpaceLength) / 2;

            this.randomGenerator = new Random();
        }

        public int MaximumRowLength
        {
            get
            {
                return horizontalSize * (averagePlarformLength + averageSpaceLength - 1);
            }
        }

        public Platform[][] Level
        {
            get
            {
                return this.level;
            }
        }

        public void GenerateInnitialLevel(int numberOfRowsToCreate)
        {
            this.level = new Platform[numberOfRowsToCreate][];

            for (int rowNumber = 0; rowNumber < numberOfRowsToCreate; rowNumber++)
            {
                Platform[] row = new Platform[this.horizontalSize * (this.averagePlarformLength + this.averageSpaceLength)];

                if (rowNumber == 0)
                {
                    for (int columnNumber = 0; columnNumber < this.MaximumRowLength; columnNumber++)
                    {
                        if (columnNumber == 0)
                        {
                            row[columnNumber] = Platform.GetRandomPlatform();
                        }
                        else if (columnNumber == this.horizontalSize - 1)
                        {
                            row[columnNumber] = row[columnNumber - 1].GetNextPlatform(row[0].LeftJoint);
                        }
                        else
                        {
                            row[columnNumber] = row[columnNumber - 1].GetNextPlatform();
                        }
                    }
                }
                else
                {
                    row = this.GenerateNewRow();
                }

                this.level[rowNumber] = row;
            }
        }

        public void GenerateNewRows(int numberOfRows)
        {
            int originalRowsCount = this.level.Count();

            Platform[][] newLevel = new Platform[originalRowsCount + numberOfRows][];

            for (int rowNumber = 0; rowNumber < originalRowsCount + numberOfRows; rowNumber++)
            {
                if (rowNumber < originalRowsCount)
                {
                    newLevel[rowNumber] = this.level[rowNumber];
                }
                else
                {
                    Platform[] row = this.GenerateNewRow();

                    newLevel[rowNumber] = row;
                }
            }

            this.level = newLevel;
        }

        private Platform[] GenerateNewRow()
        {
            List<Platform> row = new List<Platform>();

            for (int platformNumber = 0; platformNumber < this.horizontalSize; platformNumber++)
            {
                #region GeneratePlatforms
                row.Add(Platform.GetFirstPlatform());

                for (int i = 1; i < this.minimumPlatformLength; i++)
                {
                    row.Add(row.Last().GetNextPlatform());
                }

                for (int i = this.minimumPlatformLength + 1; i < this.maximumPlatformLength; i++)
                {
                    if (this.randomGenerator.Next(1, 101) <= 50)
                    {
                        row.Add(row.Last().GetNextPlatform());
                    }
                }

                row.Add(row.Last().GetNextPlatform(null));
                #endregion

                #region GenerateSpaces
                for (int i = 0; i < this.minimumSpaceLength; i++)
                {
                    row.Add(null);
                }

                for (int i = this.minimumSpaceLength; i < this.maximumPlatformLength; i++)
                {
                    if (this.randomGenerator.Next(1, 101) <= 50)
                    {
                        row.Add(null);
                    }
                }
                #endregion
            }

            //Shift the row a few blocks to the left so there wouldn't be a visible line of blocks

            int centerOfTheView = this.horizontalSize / 2;
            int numberOfBlocksToShiftToTheLeft = this.randomGenerator.Next(centerOfTheView, centerOfTheView + this.maximumPlatformLength + this.maximumSpaceLength + 1);

            List<Platform> splinterCell = row.Take(numberOfBlocksToShiftToTheLeft).ToList();

            row.AddRange(splinterCell);

            return row.Skip(numberOfBlocksToShiftToTheLeft).ToArray();
        }
    }
}
