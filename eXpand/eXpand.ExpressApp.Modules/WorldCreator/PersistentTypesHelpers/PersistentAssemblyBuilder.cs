﻿using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.ExpressApp.WorldCreator.Observers;
using eXpand.Persistent.Base.General;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Utils.ExpressionBuilder;
using eXpand.Xpo;

namespace eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers {
    public interface IClassHandler
    {
        IPersistentReferenceMemberInfo CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo referenceClassInfo, bool createAssociation);
        IPersistentReferenceMemberInfo CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name,IPersistentClassInfo referenceClassInfo);
        IPersistentReferenceMemberInfo CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, Type referenceType);
        IPersistentReferenceMemberInfo CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, Type referenceType, bool createAssociation);
        void CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc, bool createAssociation);
        void CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc);
        void CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceTypeFunc);
        void CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceClassInfoFunc, bool createAssociation);
        void CreateSimpleMembers<T>(Func<IPersistentClassInfo, IEnumerable<string>> func);
        void CreateDefaultClassOptions(Func<IPersistentClassInfo, bool> func);
        void CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo refenceClassInfo);

        void CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo refenceClassInfo,
                                    string associationName);

        void SetInheritance(Func<IPersistentClassInfo, Type> func);
        void SetInheritance(Func<IPersistentClassInfo, IPersistentClassInfo> func);
    }

    public interface IPersistentAssemblyBuilder : IClassHandler {
        IPersistentAssemblyInfo PersistentAssemblyInfo { get; }
        IClassHandler CreateClasses(IEnumerable<string> classNames);
    }

    public class PersistentAssemblyBuilder : Builder<IPersistentAssemblyInfo>, IPersistentAssemblyBuilder {
        readonly IPersistentAssemblyInfo _persistentAssemblyInfo;
        IEnumerable<IPersistentClassInfo> _persistentClassInfos;
        readonly ObjectSpace _objectSpace;

        PersistentAssemblyBuilder(IPersistentAssemblyInfo persistentAssemblyInfo)
        {
            _persistentAssemblyInfo = persistentAssemblyInfo;
            _objectSpace = ObjectSpace.FindObjectSpace(persistentAssemblyInfo);
        }

        public IPersistentAssemblyInfo PersistentAssemblyInfo
        {
            get { return _persistentAssemblyInfo; }
        }

        internal static PersistentAssemblyBuilder BuildAssembly() {
            return BuildAssembly(GetUniqueAssemblyName());
        }

        static string GetUniqueAssemblyName()
        {
            return "a" + Guid.NewGuid().ToString().Replace("-", "");
        }

        internal static PersistentAssemblyBuilder BuildAssembly(ObjectSpace objectSpace) {
            return BuildAssembly(objectSpace,GetUniqueAssemblyName());
        }
        public ObjectSpace ObjectSpace
        {
            get { return _objectSpace; }
        }
        
        internal static PersistentAssemblyBuilder BuildAssembly( string name) {
            var objectSpace = new DevExpress.ExpressApp.ObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            return BuildAssembly(objectSpace, name);
        }
        public static PersistentAssemblyBuilder BuildAssembly(ObjectSpace objectSpace, string name)
        {
            new PersistentReferenceMemberInfoObserver(objectSpace);
            new CodeTemplateInfoObserver(objectSpace);
            new CodeTemplateObserver(objectSpace);
            var assemblyInfo =
                (IPersistentAssemblyInfo) objectSpace.CreateObject(TypesInfo.Instance.PersistentAssemblyInfoType);
            assemblyInfo.Name = name;            
            return new PersistentAssemblyBuilder(assemblyInfo);
        }

        public IClassHandler CreateClasses(IEnumerable<string> classNames)
        {
            _persistentClassInfos = classNames.Select(s =>
            {
                var persistentClassInfo = (IPersistentClassInfo)_objectSpace.CreateObject(TypesInfo.Instance.PersistentTypesInfoType);
                persistentClassInfo.Name = s;
                persistentClassInfo.PersistentAssemblyInfo = _persistentAssemblyInfo;
                _persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
                persistentClassInfo.SetDefaultTemplate(TemplateType.Class);
                return persistentClassInfo;
            }).ToList();
            return this;
        }


        void IClassHandler.CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc, bool createAssociation) {
            foreach (IPersistentClassInfo info in _persistentClassInfos) {
                IEnumerable<IPersistentClassInfo> persistentClassInfos = referenceClassInfoFunc.Invoke(info);
                if (persistentClassInfos != null) {
                    foreach (IPersistentClassInfo persistentClassInfo in persistentClassInfos) {
                        ((IClassHandler) this).CreateRefenenceMember(info, persistentClassInfo.Name, persistentClassInfo,createAssociation);
                    }
                }
            }
        }

        void IClassHandler.CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc){
            ((IClassHandler) this).CreateReferenceMembers(referenceClassInfoFunc,false);
        }

        void IClassHandler.CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceTypeFunc)
        {
            CreateReferenceMembers(referenceTypeFunc, false);
        }

        public void CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceClassInfoFunc, bool createAssociation) {
            foreach (IPersistentClassInfo info in _persistentClassInfos) {
                IEnumerable<Type> types = referenceClassInfoFunc.Invoke(info);
                if (types != null) {
                    foreach (Type type in types) {
                        IPersistentReferenceMemberInfo persistentReferenceMemberInfo =
                            ((IClassHandler) this).CreateRefenenceMember(info, type.Name, type,createAssociation);
                        persistentReferenceMemberInfo.SetReferenceTypeFullName(type.FullName);
                    }
                }
            }
        }


        void IClassHandler.CreateSimpleMembers<T>(Func<IPersistentClassInfo, IEnumerable<string>> func) {
            foreach (var persistentClassInfo in _persistentClassInfos){
                IEnumerable<string> invoke = func.Invoke(persistentClassInfo);
                if (invoke!= null) {
                    foreach (string name in invoke) {
                        var persistentCoreTypeMemberInfo = (IPersistentCoreTypeMemberInfo)_objectSpace.CreateObject(TypesInfo.Instance.PersistentCoreTypeInfoType);
                        persistentCoreTypeMemberInfo.Name=name;
                        persistentClassInfo.OwnMembers.Add(persistentCoreTypeMemberInfo);
                        persistentCoreTypeMemberInfo.DataType = (XPODataType) Enum.Parse(typeof (XPODataType), typeof (T).Name);
                        persistentCoreTypeMemberInfo.SetDefaultTemplate(TemplateType.ReadWriteMember);                        
                    }
                }
            }

        }

        void IClassHandler.CreateDefaultClassOptions(Func<IPersistentClassInfo, bool> func) {
            foreach (var persistentClassInfo in _persistentClassInfos) {
                if (func.Invoke(persistentClassInfo))
                    persistentClassInfo.TypeAttributes.Add((IPersistentAttributeInfo)
                        _objectSpace.CreateObject(TypesInfo.Instance.PersistentDefaultClassOptionsAttributeType));
            }
        }

        void IClassHandler.CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name,IPersistentClassInfo refenceClassInfo) {
            ((IClassHandler) this).CreateCollectionMember(persistentClassInfo, name, refenceClassInfo, null);
        }

        void IClassHandler.CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo refenceClassInfo, string associationName) {
            var persistentCollectionMemberInfo =createPersistentAssociatedMemberInfo<IPersistentCollectionMemberInfo>(name, persistentClassInfo,
                                                                                      TypesInfo.Instance.PersistentCollectionInfoType,
                                                                                      associationName,TemplateType.ReadOnlyMember,true);
            persistentCollectionMemberInfo.SetCollectionTypeFullName(refenceClassInfo.PersistentAssemblyInfo.Name + "." +refenceClassInfo.Name);
        }

        void IClassHandler.SetInheritance(Func<IPersistentClassInfo, Type> func) {
            foreach (var persistentClassInfo in _persistentClassInfos) {
                var invoke = func.Invoke(persistentClassInfo);
                if (invoke != null) persistentClassInfo.BaseTypeFullName = invoke.FullName;
            }
        }

        void IClassHandler.SetInheritance(Func<IPersistentClassInfo, IPersistentClassInfo> func) {
            foreach (var persistentClassInfo in _persistentClassInfos) {
                var classInfo = func.Invoke(persistentClassInfo);
                if (classInfo != null)
                    persistentClassInfo.BaseTypeFullName = classInfo.PersistentAssemblyInfo.Name + "." + classInfo.Name;
            }
        }


        IPersistentReferenceMemberInfo IClassHandler.CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo referenceClassInfo, bool createAssociation) {
            var persistentReferenceMemberInfo =
                    createPersistentAssociatedMemberInfo<IPersistentReferenceMemberInfo>(name, persistentClassInfo, 
                                    TypesInfo.Instance.PersistentReferenceInfoType, TemplateType.ReadWriteMember, createAssociation);
            persistentReferenceMemberInfo.SetReferenceTypeFullName(persistentClassInfo.PersistentAssemblyInfo.Name + "." + referenceClassInfo.Name);
            return persistentReferenceMemberInfo;    
        }

        IPersistentReferenceMemberInfo IClassHandler.CreateRefenenceMember(IPersistentClassInfo info, string name,IPersistentClassInfo referenceClassInfo)
        {
            return ((IClassHandler)this).CreateRefenenceMember(info, name, referenceClassInfo, false);
        }

        IPersistentReferenceMemberInfo IClassHandler.CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, Type referenceType, bool createAssociation) {
            var persistentReferenceMemberInfo =
                createPersistentAssociatedMemberInfo<IPersistentReferenceMemberInfo>(name, persistentClassInfo,
                                                                                     TypesInfo.Instance.PersistentReferenceInfoType,
                                                                                     TemplateType.ReadWriteMember,createAssociation);
            persistentReferenceMemberInfo.SetReferenceTypeFullName(referenceType.FullName);
            return persistentReferenceMemberInfo;
        }

        IPersistentReferenceMemberInfo IClassHandler.CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, Type referenceType) {
            return ((IClassHandler) this).CreateRefenenceMember(persistentClassInfo, name, referenceType, false);
        }

        TPersistentAssociatedMemberInfo createPersistentAssociatedMemberInfo<TPersistentAssociatedMemberInfo>(
            string name, IPersistentClassInfo info, Type infoType, string assocaitionName, TemplateType templateType,
            bool createAssociationAttribute)
            where TPersistentAssociatedMemberInfo : IPersistentAssociatedMemberInfo {
            var persistentReferenceMemberInfo = (TPersistentAssociatedMemberInfo)_objectSpace.CreateObject(infoType);
            persistentReferenceMemberInfo.Name = name;
            persistentReferenceMemberInfo.RelationType=RelationType.OneToMany;
            info.OwnMembers.Add(persistentReferenceMemberInfo);
            persistentReferenceMemberInfo.SetDefaultTemplate(templateType);
            if (createAssociationAttribute){
                var persistentAssociationAttribute =
                    (IPersistentAssociationAttribute)
                    _objectSpace.CreateObject(TypesInfo.Instance.PersistentAssociationAttributeType);
                persistentAssociationAttribute.AssociationName = assocaitionName??persistentReferenceMemberInfo.Name;
                persistentReferenceMemberInfo.TypeAttributes.Add(persistentAssociationAttribute);
            }
            return persistentReferenceMemberInfo;
        }

        TPersistentAssociatedMemberInfo createPersistentAssociatedMemberInfo<TPersistentAssociatedMemberInfo>(string name, IPersistentClassInfo info, Type infoType, TemplateType templateType, bool createAssociation)
            where TPersistentAssociatedMemberInfo : IPersistentAssociatedMemberInfo {
            return createPersistentAssociatedMemberInfo<TPersistentAssociatedMemberInfo>(name, info, infoType, null, templateType,createAssociation);
        }
    }
}