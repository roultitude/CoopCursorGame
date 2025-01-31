using System;
using Unity.Behavior;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;
using System.Linq;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomCompleteAll", story: "Execute children randomly untill all succeed or fail", category: "Flow", id: "87fa570ce0fa2445ca77e868460d68f8")]
public partial class RandomCompleteAllSequence : Composite
{
    int m_CurrentIndex = -1;
    int[] m_Indexes;

    protected override Status OnStart()
    {
        if(m_CurrentIndex == -1)
        {
            m_Indexes = new int[Children.Count];
            for(int i = 0; i < m_Indexes.Length; i++)
            {
                m_Indexes[i] = i;
            }

        }
        m_CurrentIndex = 0;
        Random random = new Random(DateTime.Now.Millisecond);
        m_Indexes.OrderBy(x => random.Next()).ToArray();

        if (m_CurrentIndex < Children.Count) // incase no children
        {
            var status = StartNode(Children[m_CurrentIndex]);
            if (status == Status.Success || status == Status.Failure)
            {
                return Status.Running;
            }
            return Status.Waiting;
        }

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        var status = Children[m_CurrentIndex].CurrentStatus;
        if (status == Status.Success || status == Status.Failure)
        {
            m_CurrentIndex++;
            if(m_CurrentIndex>= Children.Count) // done
            {
                return status;
            } else //start next
            {
                var newStatus = StartNode(Children[m_CurrentIndex]);
                return Status.Running;
            }
        }
            
        return Status.Waiting;
    }

}

