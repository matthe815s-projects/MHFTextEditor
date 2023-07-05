using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Huge credit to https://github.com/Chakratos/ReFrontier/blob/master/ReFrontier/jpk/JPKDecodeLz.cs for the initial code base to work with in regards to JPK encoding.
 */
namespace MHFQuestEditor
{
    class JPKEncoder
    {
        public byte m_flag;
        public int m_shiftIndex;
        public int m_ind;
        public byte[] m_inp;
        public int m_level = 280;
        public int m_maxdist = 0x300;//0x1fff;
        public Stream m_outstream;
        public byte[] m_towrite = new byte[1000];
        public int m_itowrite;

        public void JPKEncode(byte[] data, UInt16 atype, string otPath, int level)
        {
            UInt16 type = atype;
            byte[] buffer = data;
            int insize = buffer.Length;
            if (File.Exists(otPath)) File.Delete(otPath);
            FileStream fsot = File.Create(otPath);
            BinaryWriter br = new BinaryWriter(fsot);
            UInt32 u32 = 0x1A524B4A;
            UInt16 u16 = 0x108;
            br.Write(u32);
            br.Write(u16);
            br.Write(type);
            u32 = 0x10;
            br.Write(u32);
            br.Write(insize);

            DateTime sta, fin;
            sta = DateTime.Now;
            ProcessOnEncode(buffer, fsot, level);
            fin = DateTime.Now;
            fsot.Close();
        }

        public unsafe int FindRep(int ind, out uint ofs)
        {
            int nlen = Math.Min(m_level, m_inp.Length - ind);
            ofs = 0;
            if (ind == 0 || nlen < 3)
            {
                return 0;
            }
            int ista = ind < m_maxdist ? 0 : ind - m_maxdist;
            fixed (byte* pinp = m_inp)
            {
                byte* psta = pinp + ista;
                byte* pcur = pinp + ind;
                int len = 0;
                while (psta < pcur)
                {
                    int lenw = 0;
                    byte* pfin = psta + nlen;
                    //byte* pb2;
                    for (byte* pb = psta, pb2 = pcur; pb < pfin; pb++, pb2++, lenw++)
                    {
                        if (*pb != *pb2) break;
                    }
                    if (lenw > len && lenw >= 3)
                    {
                        len = lenw;
                        ofs = (uint)(pcur - psta - 1);
                        if (len >= nlen) break;
                    }
                    psta++;
                }
                return len;
            }
        }
        public void flushflag(bool final)
        {
            if (!final || m_itowrite > 0)
                WriteByte(m_outstream, m_flag);
            m_flag = 0;
            for (int i = 0; i < m_itowrite; i++)
                WriteByte(m_outstream, m_towrite[i]);
            m_itowrite = 0;
        }
        public void SetFlag(byte b)
        {
            m_shiftIndex--;
            if (m_shiftIndex < 0)
            {
                m_shiftIndex = 7;
                flushflag(false);
            }
            m_flag |= (byte)(b << m_shiftIndex);
        }
        public void SetFlagsL(byte b, int cnt)
        {
            for (int i = cnt - 1; i >= 0; i--)
            {
                SetFlag((byte)((b >> i) & 1));
            }
        }
        public void ProcessOnEncode(byte[] inBuffer, Stream outStream, int level = 1000)
        {
            long perc, perc0 = 0;
            long percbord = 0;

            m_shiftIndex = 8;
            m_itowrite = 0;
            m_outstream = outStream;
            m_inp = inBuffer;
            m_level = level < 6 ? 6 : level > 280 ? 280 : level;
            m_maxdist = level < 50 ? 50 : level > 0x1fff ? 0x1fff : level;
            perc0 = percbord;
            m_ind = 0;
            while (m_ind < inBuffer.Length)
            {
                perc = percbord + (100 - percbord) * (Int64)m_ind / inBuffer.Length;
                if (perc > perc0)
                {
                    perc0 = perc;
                }
                uint ofs;
                int len = FindRep(m_ind, out ofs);
                //Debug.WriteLine("ind={0:x} len={1:x} ofs={2:x}",m_ind, len, ofs);
                if (len == 0)
                {
                    SetFlag(0);
                    m_towrite[m_itowrite++] = inBuffer[m_ind];
                    m_ind++;
                }
                else
                {
                    SetFlag(1);
                    if (len <= 6 && ofs <= 0xff)
                    {
                        SetFlag(0);
                        SetFlagsL((byte)((len - 3)), 2);
                        m_towrite[m_itowrite++] = (byte)ofs;
                        m_ind += len;
                    }
                    else
                    {
                        SetFlag(1);
                        UInt16 u16 = (UInt16)ofs;
                        byte hi, lo;
                        if (len <= 9) u16 |= (UInt16)((len - 2) << 13);
                        hi = (byte)(u16 >> 8);
                        lo = (byte)(u16 & 0xff);
                        m_towrite[m_itowrite++] = hi;
                        m_towrite[m_itowrite++] = lo;
                        m_ind += len;
                        if (len > 9)
                        {
                            if (len <= 25)
                            {
                                SetFlag(0);
                                SetFlagsL((byte)((len - 10)), 4);
                            }
                            else
                            {
                                SetFlag(1);
                                m_towrite[m_itowrite++] = (byte)(len - 0x1a);
                            }
                        }
                    }
                }
            }
            flushflag(true);
        }
        public void WriteByte(Stream s, byte b)
        {
            s.WriteByte(b);
        }
    }
}
