using AutoMapper;
using Contoso.Domain.Entities;
using Kendo.Mvc.Infrastructure;
using LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests.AutoMapperProfiles
{
    public class GroupingProfile : Profile
    {
        public GroupingProfile()
        {
            CreateMap<AggregateFunctionsGroup, AggregateFunctionsGroupModel<StudentModel>>()
                .ConstructUsing(ProfileHelpers.Converter<StudentModel>)
                .ForMember(d => d.Items, opt => opt.Ignore())
                .ForMember(d => d.Subgroups, opt => opt.Ignore());

            CreateMap<AggregateFunctionsGroup, AggregateFunctionsGroupModel<CourseModel>>()
                .ConstructUsing(ProfileHelpers.Converter<CourseModel>)
                .ForMember(d => d.Items, opt => opt.Ignore())
                .ForMember(d => d.Subgroups, opt => opt.Ignore());

            CreateMap<AggregateFunctionsGroup, AggregateFunctionsGroupModel<DepartmentModel>>()
                .ConstructUsing(ProfileHelpers.Converter<DepartmentModel>)
                .ForMember(d => d.Items, opt => opt.Ignore())
                .ForMember(d => d.Subgroups, opt => opt.Ignore());

            CreateMap<AggregateFunctionsGroup, AggregateFunctionsGroupModel<InstructorModel>>()
                .ConstructUsing(ProfileHelpers.Converter<InstructorModel>)
                .ForMember(d => d.Items, opt => opt.Ignore())
                .ForMember(d => d.Subgroups, opt => opt.Ignore());

            CreateMap<AggregateFunctionsGroup, AggregateFunctionsGroupModel<CourseAssignmentModel>>()
                .ConstructUsing(ProfileHelpers.Converter<CourseAssignmentModel>)
                .ForMember(d => d.Items, opt => opt.Ignore())
                .ForMember(d => d.Subgroups, opt => opt.Ignore());

            CreateMap<AggregateFunctionsGroup, AggregateFunctionsGroupModel<EnrollmentModel>>()
                .ConstructUsing(ProfileHelpers.Converter<EnrollmentModel>)
                .ForMember(d => d.Items, opt => opt.Ignore())
                .ForMember(d => d.Subgroups, opt => opt.Ignore());
        }
    }

    internal static class ProfileHelpers
    {//AggregateFunctionsGroup.Items are a list of TData items.
        public static AggregateFunctionsGroupModel<T> Converter<T>(AggregateFunctionsGroup src, ResolutionContext context)
        {
            System.Collections.IEnumerable items = null;
            if (src.Items != null && src.Items.GetType().UnderlyingElementTypeIsFunctionsGroup())
                items = context.Mapper.Map<IEnumerable<AggregateFunctionsGroupModel<T>>>(src.Items);
            else
                items = context.Mapper.Map<IEnumerable<T>>(src.Items);

            return new AggregateFunctionsGroupModel<T> { Items = items };
        }

        private static bool UnderlyingElementTypeIsFunctionsGroup(this Type type)
        {
            Type[] genericArguments;
            if (!type.IsGenericType || (genericArguments = type.GetGenericArguments()).Length != 1)
                return false;

            return genericArguments[0] == typeof(AggregateFunctionsGroup);
        }
    }
}
