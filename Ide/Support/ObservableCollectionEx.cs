////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  Implements the observable collection ex class
//
// Author:
//  Robert C. Steiner
//
// History:
//  =====================================================================================================
//     Date      Who                           What
//  ----------- ------   --------------------------------------------------------------------------------
//   6/16/2015   rcs     Initial Implementation
//  =====================================================================================================
//
//  BSD 3-Clause License
//  Copyright (c) 2020, Robert C. Steiner
//  All rights reserved.
//  Redistribution and use in source and binary forms, with or without   modification,
//  are permitted provided that the following conditions are met:
//
//  1. Redistributions of source code must retain the above copyright notice, this
//  list of conditions and the following disclaimer.
//
//  2. Redistributions in binary form must reproduce the above copyright notice,
//  this list of conditions and the following disclaimer in the documentation
//  and/or other materials provided with the distribution.
//
//  3. Neither the name of the copyright holder nor the names of its
//  contributors may be used to endorse or promote products derived from
//  this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
//  FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//  DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
//  CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//  OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ide
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Observable collection ex. 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ObservableCollectionEx()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="items">    The items. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ObservableCollectionEx(List<T> items) : base(items) { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="items">    The items. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ObservableCollectionEx(IEnumerable<T> items) : base(items) { }

        #region IndexOf

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns the index of the first object which meets the specified function.
        /// </summary>
        /// <param name="compareFunction">  A bool function to compare each Item by. </param>
        /// <returns>
        /// The index of the first Item which matches the function.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int IndexOf(Func<T, bool> compareFunction)
        {
            return Items.IndexOf(Items.FirstOrDefault(compareFunction));
        }

        #endregion

        #region Sorting

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sorts the items of the collection in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey"> The type of the key returned by <paramref name="keySelector"/>. </typeparam>
        /// <param name="keySelector">  A function to extract a key from an item. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Sort<TKey>(Func<T, TKey> keySelector)
        {
            InternalSort(Items.OrderBy(keySelector));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sorts the items of the collection in descending order according to a key.
        /// </summary>
        /// <typeparam name="TKey"> The type of the key returned by <paramref name="keySelector"/>. </typeparam>
        /// <param name="keySelector">  A function to extract a key from an item. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SortDescending<TKey>(Func<T, TKey> keySelector)
        {
            InternalSort(Items.OrderByDescending(keySelector));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sorts the items of the collection in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey"> The type of the key returned by <paramref name="keySelector"/>. </typeparam>
        /// <param name="keySelector">  A function to extract a key from an item. </param>
        /// <param name="comparer">     An <see cref="IComparer{T}"/> to compare keys. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            InternalSort(Items.OrderBy(keySelector, comparer));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Moves the items of the collection so that their orders are the same as those of the items provided.
        /// </summary>
        /// <param name="sortedItems">  An <see cref="IEnumerable{T}"/> to provide item orders. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void InternalSort(IEnumerable<T> sortedItems)
        {
            var sortedItemsList = sortedItems.ToList();

            foreach (var item in sortedItemsList)
            {
                Move(IndexOf(item), sortedItemsList.IndexOf(item));
            }
        }

  #endregion
    }
}