using Unity.Collections;
using Unity.Netcode;

namespace Kraft.Utils
{
    /// <summary>
    /// Wrapping FixedString so that if we want to change player name max size in the future, we only do it once here
    /// </summary>
    public struct FixedPlayerName : INetworkSerializable
    {
        FixedString32Bytes m_Name;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref m_Name);
        }

        public override string ToString()
        {
            return m_Name.ToString();
        }

        public FixedPlayerName(string name)
        {
            m_Name = new FixedString32Bytes(name);
        }

        public static implicit operator FixedPlayerName(string name) => new FixedPlayerName(name);
    }
}
