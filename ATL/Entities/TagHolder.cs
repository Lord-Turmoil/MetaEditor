﻿
namespace ATL.AudioData.IO
{
    /// <summary>
    /// Represents a set of metadata
    /// </summary>
    public class TagHolder : MetaDataHolder
    {
        private readonly MetaDataIOFactory.TagType tagType = MetaDataIOFactory.TagType.ANY;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TagHolder()
        {
            tagData = new TagData();
        }

        /// <summary>
        /// Instanciate a new TagHolder populated with the given TagData
        /// </summary>
        /// <param name="tagData">Data to use to populate the new instance</param>
        public TagHolder(TagData tagData)
        {
            this.tagData = new TagData(tagData);
        }

        /// <inheritdoc/>
        protected override MetaDataIOFactory.TagType getImplementedTagType()
        {
            return tagType;
        }
    }
}
