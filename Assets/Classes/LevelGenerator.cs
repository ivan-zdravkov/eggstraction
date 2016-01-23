using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    public class LevelGenerator
    {
        private int horizontalSize;
        private int maxDeviationPercentage;

        private int minimumPlatformLength;
        private int maximumPlatformLength;
        private int averagePlarformLength;

        private int minimumSpaceLength;
        private int maximumSpaceLength;
        private int averageSpaceLength;

        private Random randomGenerator;
        private Platform[][] level;
        
        public LevelGenerator(int horizontalSize, int maxDeviationPercentage, int minimumPlatformLength, int maximumPlatformLength, int minimumSpaceLength, int maximumSpaceLength)
        {
            if ((minimumPlatformLength + maximumPlatformLength) % 2 != 0)
            {
                throw new ArgumentException("The minimumPlatformLength and maximumPlatformLength must have an integer averagePlarformLength between them.");
            }

            if ((minimumSpaceLength + maximumSpaceLength) % 2 != 0)
            {
                throw new ArgumentException("The minimumSpaceLength and maximumSpaceLength must have an integer averageSpaceLength between them.");
            }

            this.horizontalSize = horizontalSize;
            this.maxDeviationPercentage = maxDeviationPercentage;

            this.minimumPlatformLength = minimumPlatformLength;
            this.maximumPlatformLength = maximumPlatformLength;
            this.averagePlarformLength = (minimumPlatformLength + maximumPlatformLength) / 2;

            this.minimumSpaceLength = minimumSpaceLength;
            this.maximumSpaceLength = maximumSpaceLength;
            this.averageSpaceLength = (minimumSpaceLength + maximumSpaceLength) / 2;

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
            int totalDeviation = this.horizontalSize % (this.averagePlarformLength + this.averageSpaceLength);

            int platformDeviation = -totalDeviation / 2;
            int spaceDeviation = -(totalDeviation + platformDeviation);

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
                        if (columnNumber == this.horizontalSize - 1 - this.minimumSpaceLength)
                        {
                            row[columnNumber] = row[columnNumber - 1].GetNextPlatform(null);

                            isPlatformMode = false;

                            continue;
                        }

                        row[columnNumber] = row[columnNumber - 1].GetNextPlatform();

                        int numberOfConsecutivePlatforms = 2;

                        while (columnNumber - numberOfConsecutivePlatforms > -1 && row[columnNumber - numberOfConsecutivePlatforms] != null)
                        {
                            numberOfConsecutivePlatforms++;
                        }

                        if (numberOfConsecutivePlatforms < this.minimumPlatformLength - 1)
                        {
                            continue;
                        }

                        if (numberOfConsecutivePlatforms == this.maximumPlatformLength - 1)
                        {
                            isLastPlatform = true;
                            continue;
                        }

                        numberOfConsecutivePlatforms += 1; // Adding for the last platform we know for sure exists!

                        bool addNextPlatform = false;

                        int currentPlatformDeviation = platformDeviation + numberOfConsecutivePlatforms - this.averagePlarformLength;
                        int percentageThreshhold = 50 + ((currentPlatformDeviation * 5 <= this.maxDeviationPercentage) ? (currentPlatformDeviation * 5) : this.maxDeviationPercentage);

                        int randomPercentageNumber = this.randomGenerator.Next(0, 101);

                        if (randomPercentageNumber > percentageThreshhold)
                        {
                            addNextPlatform = true;
                        }

                        if (addNextPlatform)
                        {
                            if (numberOfConsecutivePlatforms > this.averagePlarformLength)
                            {
                                platformDeviation--;
                            }

                            continue;
                        }
                        else
                        {
                            if (numberOfConsecutivePlatforms < this.averagePlarformLength)
                            {
                                platformDeviation++;

                                isLastPlatform = true;
                            }

                            continue;
                        }
                    }
                }
                else
                {
                    row[columnNumber] = null;

                    int numberOfConsecutiveSpaces = 1;

                    while (row[columnNumber - numberOfConsecutiveSpaces] == null)
                    {
                        numberOfConsecutiveSpaces++;
                    }

                    if (numberOfConsecutiveSpaces < this.minimumSpaceLength)
                    {
                        continue;
                    }

                    int maxRemainingSpacesToAdd = this.maximumSpaceLength - numberOfConsecutiveSpaces;

                    if (columnNumber + maxRemainingSpacesToAdd >= this.horizontalSize - 1)
                    {
                        continue; // Keep adding spaces until the end.
                    }

                    if (numberOfConsecutiveSpaces == this.maximumSpaceLength)
                    {
                        isPlatformMode = true;
                        isFirstPlatform = true;
                        isLastPlatform = false;

                        continue;
                    }

                    bool addNextSpace = false;

                    int currentSpaceDeviation = spaceDeviation + numberOfConsecutiveSpaces - this.averagePlarformLength;
                    int percentageThreshhold = 50 + ((currentSpaceDeviation * 5 <= this.maxDeviationPercentage) ? (currentSpaceDeviation * 5) : this.maxDeviationPercentage);

                    int randomPercentageNumber = this.randomGenerator.Next(0, 101);

                    if (randomPercentageNumber > percentageThreshhold)
                    {
                        addNextSpace = true;
                    }

                    if (addNextSpace)
                    {
                        if (numberOfConsecutiveSpaces > this.averageSpaceLength)
                        {
                            spaceDeviation++;
                        }

                        continue;
                    }
                    else
                    {
                        if (numberOfConsecutiveSpaces < this.averageSpaceLength)
                        {
                            spaceDeviation--;

                            isPlatformMode = true;
                            isFirstPlatform = true;
                            isLastPlatform = false;
                        }

                        continue;
                    }
                }
            }
        }
    }
}
