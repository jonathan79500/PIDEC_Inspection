import { Contact } from './Contact.model';
import { Project } from "./Project.model";

export interface ProjectContact{

  id:string,
  projectId:string,
  contact:Contact
}
