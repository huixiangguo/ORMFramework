using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public enum Fetch
    {
        /// <summary>
        /// Ĭ��ֵ,��ʵ����������������õ�Ϊ׼
        /// </summary>
        Default=0,
        /// <summary>
        /// һ�ɲ��������úͼ�������
        /// </summary>
        NONE=1,
        /// <summary>
        /// ���ض�������,�����ؼ���
        /// </summary>
        REFS=2,
        /// <summary>
        /// ���ؼ��ϲ����ض�������
        /// </summary>
        SETS=3,
        /// <summary>
        /// ���ض������úͼ���
        /// </summary>
        REFSandSets=4
    }
}
