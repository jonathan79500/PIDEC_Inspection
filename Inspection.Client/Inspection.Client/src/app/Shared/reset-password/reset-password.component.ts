import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { CommonModule } from '@angular/common';
import { UserService } from '../../Core/Services/user.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-reset-password',
  imports: [ReactiveFormsModule,CommonModule,FormsModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit {

  fgResetPassword : FormGroup;
 userId: string = '';
  token: string = '';
constructor(private route: ActivatedRoute,private router: Router,private fBuilder:FormBuilder,private userService: UserService,private toastr:ToastrService) {}

  ngOnInit(): void {
     this.fgResetPassword =this.fBuilder.group({
      UserId: [''],
      Token: [''],
      newPass: ['', [Validators.required, Validators.minLength(8)]],
      newPassConfirm : ['', Validators.required]
     },{ validator: this.passwordsMatchValidator })

    this.route.queryParams.subscribe(params => {
      this.userId = params['userId'];
      this.token = params['token'];
    });
  }

  passwordsMatchValidator(form: FormGroup) {
  return form.get('newPass')?.value === form.get('newPassConfirm')?.value
    ? null : { mismatch: true };
}

  onSubmit():void{
    var newPassformValue = this.fgResetPassword.getRawValue();

    this.userService.resetPassword(this.token,this.userId,newPassformValue.newPass,newPassformValue.newPassConfirm).subscribe((result)=>
    {
      if(result == true)
      {
        this.toastr.success("Reset Password Successfully done!!!!!");
        this.router.navigate(['login']);
      }
      else
      {
        this.toastr.error("Reset Password Failed!!!!!");
      }
    });
  }



}
