// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

// Copyright (c) 2004 Mainsoft Co.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Xunit;


namespace System.Data.Tests
{
    public class InvalidConstraintExceptionTest
    {
        [Fact]
        public void Generate()
        {
            DataTable dtParent;
            dtParent = DataProvider.CreateParentDataTable();
            Exception tmpEx = new Exception();

            //------ check ForeignKeyConstraint  ---------
            DataTable dtChild = DataProvider.CreateChildDataTable();
            var ds = new DataSet();
            ds.Tables.Add(dtChild);
            ds.Tables.Add(dtParent);

            ds.Relations.Add(new DataRelation("myRelation", dtParent.Columns[0], dtChild.Columns[0], true));

            //update to value which is not exists in Parent table
            // InvalidConstraintException - update child row
            Assert.Throws<InvalidConstraintException>(() =>
            {
                dtChild.Rows[0]["ParentId"] = 99;
            });

            //Add another relation to the same column of the existing relation in child table
            // InvalidConstraintException - Add Relation Child
            Assert.Throws<InvalidConstraintException>(() =>
            {
                ds.Relations.Add(new DataRelation("test", dtParent.Columns[2], dtChild.Columns[0], true));
            });

            //Attempt to clear rows from parent table 
            // InvalidConstraintException - RowsCollection.Clear
            Assert.Throws<InvalidConstraintException>(() =>
            {
                dtParent.Rows.Clear();
            });

            //try to run commands on two different datasets
            DataSet ds1 = new DataSet();
            ds1.Tables.Add(dtParent.Copy());

            // InvalidConstraintException - Add relation with two DataSets
            Assert.Throws<InvalidConstraintException>(() =>
            {
                ds.Relations.Add(new DataRelation("myRelation", ds1.Tables[0].Columns[0], dtChild.Columns[0], true));
            });
        }
    }
}
