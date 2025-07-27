using Shared.DTOs.TransactionsDTOs;
using Shared.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class BuyAndSellTransactionList : IList<BuyAndSellTransaction>
    {

        #region Ctor

        public BuyAndSellTransactionList(List<BuyAndSellTransaction> list)
        {
            _list = list;
        }

        #endregion

        #region Private Fields

        private List<BuyAndSellTransaction> _list;

        #endregion

        #region Public Methods

        public async Task<IEnumerable<BuyAndSellTransactionDTO>> GetConvertedDTOValues()
        {
            var List = new List<BuyAndSellTransactionDTO>();

            foreach (BuyAndSellTransaction item in _list)
            {
                List.Append(item.ToBuyAndSellTransactionDTO());

            }
            return List;

        }

        #endregion

        #region Interface Implements

        public int Count => ((ICollection<BuyAndSellTransaction>)_list).Count;

        public bool IsReadOnly => ((ICollection<BuyAndSellTransaction>)_list).IsReadOnly;

        public BuyAndSellTransaction this[int index] { get => ((IList<BuyAndSellTransaction>)_list)[index]; set => ((IList<BuyAndSellTransaction>)_list)[index] = value; }

        public int IndexOf(BuyAndSellTransaction item)
        {
            return ((IList<BuyAndSellTransaction>)_list).IndexOf(item);
        }

        public void Insert(int index, BuyAndSellTransaction item)
        {
            ((IList<BuyAndSellTransaction>)_list).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<BuyAndSellTransaction>)_list).RemoveAt(index);
        }

        public void Add(BuyAndSellTransaction item)
        {
            ((ICollection<BuyAndSellTransaction>)_list).Add(item);
        }

        public void Clear()
        {
            ((ICollection<BuyAndSellTransaction>)_list).Clear();
        }

        public bool Contains(BuyAndSellTransaction item)
        {
            return ((ICollection<BuyAndSellTransaction>)_list).Contains(item);
        }

        public void CopyTo(BuyAndSellTransaction[] array, int arrayIndex)
        {
            ((ICollection<BuyAndSellTransaction>)_list).CopyTo(array, arrayIndex);
        }

        public bool Remove(BuyAndSellTransaction item)
        {
            return ((ICollection<BuyAndSellTransaction>)_list).Remove(item);
        }

        public IEnumerator<BuyAndSellTransaction> GetEnumerator()
        {
            return ((IEnumerable<BuyAndSellTransaction>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        #endregion

    }
}
