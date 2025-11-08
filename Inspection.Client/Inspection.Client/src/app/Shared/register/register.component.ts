import { CommonModule } from '@angular/common';
import { Component, ViewEncapsulation } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators ,ValidatorFn, AbstractControl} from '@angular/forms';
import { getMatIconFailedToSanitizeUrlError } from '@angular/material/icon';
import { FirstKeyPipe } from '../Pipes/first-key.pipe';
import { UserService } from '../../Core/Services/user.service';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-register',
    imports: [CommonModule, ReactiveFormsModule, FirstKeyPipe],
    templateUrl: './register.component.html',
    styleUrl: './register.component.css',
    encapsulation: ViewEncapsulation.None
})
export class RegisterComponent {

  isSubmitted :boolean= false;
 
  constructor(public formBuilder:FormBuilder,private userService:UserService,private toastr:ToastrService) {   
    
  }

  passwordMatchValidator :ValidatorFn= (control:AbstractControl):null=>{ 
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if(password && confirmPassword && password.value != confirmPassword.value)
      confirmPassword?.setErrors({passwordMismatch : true})
    else
      confirmPassword?.setErrors(null);
    return null;
  }

  form = this.formBuilder.group({

    fullName:['',Validators.required],
    email:['',[Validators.required,Validators.email]],
    password:['',
      [
        Validators.required,
        Validators.minLength(6),
        Validators.pattern(/(?=.*[^a-zA-Z0-9])/)]],
    confirmPassword:['']
  },
   {validators:this.passwordMatchValidator}
  );

  hasDisplayableError(controlName :string):boolean
  {
    const control = this.form.get(controlName);
    return Boolean(control?.invalid ) && (this.isSubmitted && Boolean(control?.touched));
  }
  onSubmit(){
    if(this.form.valid)
      this.userService.CreateUser(this.form.value).subscribe(
        {
          next:res =>{
            this.isSubmitted = true;
            this.toastr.success('New User Created!','Registeration Successful');
            console.log(res);
          },
          error:err=>console.log('error in create user:',err)

        }
      );

  }
}
