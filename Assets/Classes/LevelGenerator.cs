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
        private float blockEmptySpaceRatio;

        private Random randomGenerator;
        private Platform[][] level;
        
        public LevelGenerator(float blockEmptySpaceRatio, int horizontalSize, int minimumPlatformLength, int maximumPlatformLength)
        {
            if ((minimumPlatformLength + maximumPlatformLength) % 2 != 0)
            {
                throw new ArgumentException("The minimumPlatformLength and maximumPlatformLength must have an integer averagePlarformLength between them.");
            }

            this.blockEmptySpaceRatio = blockEmptySpaceRatio;
            this.horizontalSize = horizontalSize;
            this.minimumPlatformLength = minimumPlatformLength;
            this.maximumPlatformLength = maximumPlatformLength;
            this.averagePlarformLength = (minimumPlatformLength + maximumPlatformLength) / 2;

            this.randomGenerator = new Random();
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
                Platform[] row = new Platform[this.horizontalSize];

                if (rowNumber == 0)
                {
                    for (int columnNumber = 0; columnNumber < this.horizontalSize; columnNumber++)
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
                    this.PopulateNewRow(row);
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
                    Platform[] row = new Platform[this.horizontalSize];

                    this.PopulateNewRow(row);

                    newLevel[rowNumber] = row;
                }
            }

            this.level = newLevel;
        }

        private void PopulateNewRow(Platform[] row)
        {
            int absoluteNumberOfPlatformsInLevel = (int)(this.horizontalSize * this.blockEmptySpaceRatio);
            int absoluteNumberOfSpacesInLevel = this.horizontalSize - absoluteNumberOfPlatformsInLevel;

            // Generate no more than 10% of the total platform count as random deviation with a random sign;
            int randomPlatformSpaceModifier = (int)(this.randomGenerator.Next(0, absoluteNumberOfPlatformsInLevel / 10) * Math.Pow((double)-1, (double)this.randomGenerator.Next(1, 3)));

            absoluteNumberOfPlatformsInLevel += randomPlatformSpaceModifier;
            absoluteNumberOfSpacesInLevel -= randomPlatformSpaceModifier;

            int numberOfPlatformSpacePairs = absoluteNumberOfPlatformsInLevel / this.averagePlarformLength;

            int platformDeiviation = absoluteNumberOfPlatformsInLevel % this.averagePlarformLength;
            int spaceDeviation = -platformDeiviation;

            bool isPlatformMode = true;
            bool isFirstPlatform = true;
            bool isLastPlatform = false;

            for (int columnNumber = 0; columnNumber < this.horizontalSize; columnNumber++)
            {
                if (isPlatformMode)
                {
                    if (isFirstPlatform)
                    {
                        row[columnNumber] = Platform.GetFirstPlatform();

                        isFirstPlatform = false;
                    }
                    else if (isLastPlatform)
                    {
                        row[columnNumber] = row[columnNumber - 1].GetNextPlatform(null);

                        isPlatformMode = false;
                    }
                    else
                    {
                        row[columnNumber] = row[columnNumber - 1].GetNextPlatform();

                        //TODO: set isLastPlatform following the deviation
                        //isLastPlatform = true;
                    }
                }
                else
                {
                    row[columnNumber] = null;

                    //TODO: set isPlatformMode following the deviation
                    //isPlatformMode = true;
                    //isFirstPlatform = true;
                    //isLastPlatform = false;
                }
            }
        }
    }
}
