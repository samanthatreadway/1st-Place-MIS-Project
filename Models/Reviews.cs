using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fa25group23final.Models
{
    public class Reviews
    {

        [Key]
        public int ReviewID { get; set; } //Primary Key for Review Class

        //Scalar Properties
        [Range(1, 5)]
        [Display(Name = "Rating:")]
        public int Rating { get; set; } //Rating is score of book user inputted


        [Display(Name = "Comments:")]
        [StringLength(100, ErrorMessage = "Comment cannot exceed 100 characters")]
        public string ReviewText { get; set; } //ReviewText: string of text that is body of the review


        // TODO: what happens to pending reviews, like it hasn't been approved or rejected yet? should dispute status be nullable?
        // I just made it nullable, so if disputeStatus = null, then it's pending employee approval
        [Display(Name = "Approval:")]
        public bool? DisputeStatus { get; set; } //Dispute status is bool whether the review was approved/rejected by admin/employee
                                                 // True = accepted , False = rejected

        //Navigational Properties

        // foreign key explicit
        public string reviewerID { get; set; }
        public string? approverID { get; set; }

        //ReviewerID
        public AppUser reviewer { get; set; } //reivewerID is the custoemr ID from app user that writes the review

        //ApproverID
        public AppUser? approver { get; set; } //approverID is admin/Employee appuser ID who approves the review

        //BookID
        public Books? bookID { get; set; } //BookID relates to the book the review is over
        
    }
}

