﻿/*
 * Copyright 2010-2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.Runtime;
using Amazon.EC2.Model;
using System.IO;
using Amazon.Util;
using Amazon.Runtime.Internal.Util;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;

namespace Amazon.EC2.Internal
{
    public class AmazonEC2ResponseHandler : GenericHandler
    {
        protected override void PostInvoke(IExecutionContext executionContext)
        {
            var response = executionContext.ResponseContext.Response;
            var webResponseData = executionContext.ResponseContext.HttpResponse;

            var dsirr = response as DescribeSpotInstanceRequestsResult;
            if (dsirr != null)
            {
                if (dsirr.SpotInstanceRequests != null)
                {
                    foreach (var spotInstanceRequest in dsirr.SpotInstanceRequests)
                    {
                        var launchSpecification = spotInstanceRequest.LaunchSpecification;
                        PopulateLaunchSpecificationSecurityGroupNames(launchSpecification);
                    }
                }
                return;
            }

            var rsir = response as RequestSpotInstancesResult;
            if (rsir != null)
            {
                if (rsir.SpotInstanceRequests != null)
                {
                    foreach (var spotInstanceRequest in rsir.SpotInstanceRequests)
                    {
                        var launchSpecification = spotInstanceRequest.LaunchSpecification;
                        PopulateLaunchSpecificationSecurityGroupNames(launchSpecification);
                    }
                }
                return;
            }

            var dir = response as DescribeInstancesResult;
            if (dir != null)
            {
                if (dir.Reservations != null)
                {
                    foreach (var reservation in dir.Reservations)
                    {
                        PopulateReservationSecurityGroupNames(reservation);
                    }
                }
                return;
            }

            var rir = response as RunInstancesResult;
            if (rir != null)
            {
                PopulateReservationSecurityGroupNames(rir.Reservation);
                return;
            }
        }

        private static void PopulateLaunchSpecificationSecurityGroupNames(LaunchSpecification launchSpecification)
        {
            if (launchSpecification != null)
            {
                var groupNames = new List<string>();
                foreach (GroupIdentifier group in launchSpecification.AllSecurityGroups)
                {
                    groupNames.Add(group.GroupName);
                }
                launchSpecification.SecurityGroups = groupNames;
            }
        }

        private static void PopulateReservationSecurityGroupNames(Reservation reservation)
        {
            if (reservation != null)
            {
                var groupNames = new List<string>();
                foreach (GroupIdentifier group in reservation.Groups)
                {
                    groupNames.Add(group.GroupName);
                }
                reservation.GroupNames = groupNames;
            }
        }
    }
}
