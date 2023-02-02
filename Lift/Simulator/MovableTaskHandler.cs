////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   Display change state handler delegate
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/8/2018   RCS       Initial code.
//  
// 
//=====================================================[ Copyright ]=====================================================
// 
//  Copyright Advanced Performance
//  All Rights Reserved THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Advanced Performance
//  The copyright notice above does not evidence any actual or intended publication of such source code.
//  Some third-party source code components may have been modified from their original versions
//  by Advanced Performance. The modifications are Copyright Advanced Performance., All Rights Reserved.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace WPFElevator
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Delegate: Movable task
    /// </summary>
    /// <param name="deltaTime"> The delta Time.</param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public delegate void MovableTaskHandler(double deltaTime);
}