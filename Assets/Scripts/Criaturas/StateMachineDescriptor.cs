using System;
using Unity.VisualScripting; // Certifique-se de que este using está presente, se necessário

namespace Unity.VisualScripting
{
    // Definição mínima do atributo Descriptor
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptorAttribute : Attribute
    {
        public DescriptorAttribute(Type type) { }
    }

    // Definição mínima de StateMachine
    public class StateMachine { }

    // Definição mínima de MachineDescription
    public class MachineDescription { }

    // Definição mínima de MachineDescriptor
    public class MachineDescriptor<T1, T2>
    {
        public MachineDescriptor(T1 target) { }
    }

    [Descriptor(typeof(StateMachine))]
    public sealed class StateMachineDescriptor : MachineDescriptor<StateMachine, MachineDescription>
    {
        public StateMachineDescriptor(StateMachine target) : base(target) { }
    }
}
